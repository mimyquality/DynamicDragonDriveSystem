/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    //using VRC.Udon;
    using VRC.SDK3.Components;

    public enum DragonDriverStateType
    {
        Walking,
        Hovering,
        Flight,
        Brakes,
        Overdrive
    }
    public enum DragonDriverEnabledStateSelect
    {
        Landing = 1 << 0,
        Hovering = 1 << 1,
        Flight = 1 << 2
    }

    [AddComponentMenu("Dynamic Dragon Drive System/Dragon Driver")]
    [RequireComponent(typeof(Rigidbody), typeof(VRCObjectSync), typeof(SphereCollider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class DragonDriver : UdonSharpBehaviour
    {
        [Header("Speed settings")]
        [SerializeField, Tooltip("m/s^2")]
        private float _acceleration = 5.0f;
        [SerializeField, Tooltip("m/s")]
        private float _maxSpeed = 56.0f;
        [SerializeField, Tooltip("m/s")]
        private float _maxWalkSpeed = 16.0f;
        [SerializeField, Tooltip("m/s")]
        private float _hoveringSpeedThreshold = 12.0f;
        [SerializeField, Tooltip("m/s")]
        private float _stillSpeedThreshold = 3.0f;
        [SerializeField, Min(0.0f)]
        private float _stillDrag = 1.0f;

        [Header("Rotate settings")]
        [SerializeField, Tooltip("deg/s")]
        private float _updownSpeed = 45.0f;
        [SerializeField, Tooltip("deg/s")]
        private float _rollSpeed = 45.0f;
        [SerializeField, Range(0.0f, 2.0f)]
        private float _rollToTurnRatio = 1.0f;
        [SerializeField, Tooltip("sec"), Min(0.0f)]
        private float _inertialInputDuration = 0.3f;
        [SerializeField, Tooltip("deg/s")]
        private float _noseRotateSpeed = 90.0f;
        [SerializeField, Tooltip("degree"), Range(0.0f, 89.0f)]
        private float _maxNosePitch = 89.0f;
        [SerializeField, Tooltip("degree"), Range(0.0f, 89.0f)]
        private float _maxNoseYaw = 45.0f;
        [SerializeField, Tooltip("degree"), Range(0.0f, 89.0f)]
        private float _centerSnapTolerance = 5.0f;
        [SerializeField, Tooltip("deg/s")]
        private float _stateShiftSpeed = 20.0f;
        [SerializeField, Tooltip("deg/s")]
        private float _landingSpeed = 60.0f;

        [Header("Others")]
        [SerializeField, EnumFlag]
        private DragonDriverEnabledStateSelect _enabledState = (DragonDriverEnabledStateSelect)
        (
            (1 << 0) |  // Landing
            (1 << 1) |  // Hovering
            (1 << 2)    // Flight
        );
        [SerializeField, Tooltip("m/s"), Min(0.0f)]
        private float _accelerateLimit = 5.0f;
        [SerializeField, Tooltip("m/s")]
        private float _jumpImpulse = 8.0f;
        [SerializeField, Min(0.0f)]
        private float _brakePower = 2.0f;
        [SerializeField]
        private LayerMask _groundLayer =
         (1 << 0) |  // Default
         (1 << 1) |  // TransparentFX
         (1 << 2) |  // Ignore Raycast
         (1 << 4) |  // Water
         (1 << 8) |  // Interactive
         (1 << 11) | // Environment
         (1 << 15) | // StereoLeft
         (1 << 16) | // StereoRight
         (1 << 17);  // Walkthrough
        [SerializeField, Tooltip("degree"), Range(0.0f, 89.9f)]
        private float _slopeLimit = 45.0f;


        [UdonSynced(UdonSyncMode.Linear)]
        private Vector3 sync_velocity;
        [UdonSynced(UdonSyncMode.Linear)]
        private Vector3 sync_angularVelocity;

        // コンポーネント
        private Rigidbody _rigidbody;
        private SphereCollider _collider;
        private VRCObjectSync _objectSync;

        // 計算用
        private bool _isSleeping = true;
        private Vector3 _velocity, _targetVelocity;
        private Quaternion _rotation, _noseRotation;
        private float _drag, _defaultDrag, _sqrSpeed;
        private DragonDriverStateType _state;
        private bool _isWalking, _isGrounded, _isBrakes, _isOverdrive;
        private float _inertialRoll, _inertialPitch;
        private Vector3 _colliderCenter;
        private float _groundCheckRadius, _groundCheckRange;
        private RaycastHit _groundInfo = new RaycastHit();

        // Input
        private bool _isDrive;
        private Vector3 _throttle;
        private float _elevator, _ladder, _aileron;
        private Vector3 _directRotation = Vector3.zero;
        private Quaternion _gazeRotation = Quaternion.identity;

        // Actor渡し用
        public bool IsGrounded { get => _isGrounded; }
        public bool IsBrakes { get => _isBrakes; }
        public bool IsOverdrive { get => _isOverdrive; }
        public int State { get => (int)_state; }
        public Vector3 Velocity { get => _isSleeping ? Vector3.zero : sync_velocity; }
        public Vector3 AngularVelocity { get => _isSleeping ? Vector3.zero : sync_angularVelocity; }
        public Vector3 NoseDirection { get => Quaternion.Inverse(_rotation) * _noseRotation * Vector3.forward; }

        // Saddle受け取り用
        public bool IsDrive
        {
            get => _isDrive;
            set
            {
                Initialize();

                if (!value)
                {
                    _InputAccelerate(Vector3.zero);
                    _InputRotate(Vector3.zero);
                    _InputDirectRotate(Vector3.zero);
                    _InputGazeRotate(Quaternion.identity);
                    _InputEmergencyBrakes(false);
                    _InputOverdrive(false);

                    var horizontalForward = Vector3.ProjectOnPlane(_rotation * Vector3.forward, Vector3.up);
                    _rotation = Quaternion.LookRotation(horizontalForward);
                    _rigidbody.rotation = _rotation;

                    _targetVelocity = Vector3.zero;
                    _rigidbody.velocity = Vector3.zero;
                }

                _isDrive = value;
            }
        }

        // パラメーター後調整用
        public float Acceleration { get => _acceleration; set => _acceleration = Mathf.Max(value, 0.0f); }
        public float MaxSpeed { get => _maxSpeed; set => _maxSpeed = Mathf.Max(value, 0.0f); }
        public float MaxWalkSpeed { get => _maxWalkSpeed; set => _maxWalkSpeed = Mathf.Max(value, 0.0f); }
        public float HoveringSpeedThreshold { get => _hoveringSpeedThreshold; set => _hoveringSpeedThreshold = Mathf.Max(value, 0.0f); }
        public float UpdownSpeed { get => _updownSpeed; set => _updownSpeed = Mathf.Max(value, 0.0f); }
        public float RollSpeed { get => _rollSpeed; set => _rollSpeed = Mathf.Max(value, 0.0f); }
        public float RollToTurnRatio { get => _rollToTurnRatio; set => _rollToTurnRatio = Mathf.Clamp(value, 0.0f, 2.0f); }
        public float NoseRotateSpeed { get => _noseRotateSpeed; set => _noseRotateSpeed = Mathf.Max(value, 0.0f); }
        public float MaxNosePitch { get => _maxNosePitch; set => _maxNosePitch = Mathf.Clamp(value, 0.0f, 89.0f); }
        public float MaxNoseYaw { get => _maxNoseYaw; set => _maxNoseYaw = Mathf.Clamp(value, 0.0f, 89.0f); }
        public float JumpImpulse { get => _jumpImpulse; set => _jumpImpulse = Mathf.Max(value, 0.0f); }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();
            _objectSync = GetComponent<VRCObjectSync>();

            _rigidbody.freezeRotation = true;
            _rigidbody.maxDepenetrationVelocity = _maxSpeed;
            _objectSync.AllowCollisionOwnershipTransfer = false;
            _objectSync.SetKinematic(false);
            _objectSync.SetGravity(false);

            _defaultDrag = _rigidbody.drag;
            _colliderCenter = _collider.center;
            _groundCheckRadius = _collider.radius * 0.9f;
            _groundCheckRange = 2 * (_collider.radius - _groundCheckRadius);

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        private void FixedUpdate()
        {
            // ローカル処理
            _isSleeping = _rigidbody.IsSleeping();
            _isGrounded = CheckGrounded();

            if (!Networking.IsOwner(this.gameObject)) { return; }

            // 現状を算出
            _velocity = _rigidbody.velocity;
            _sqrSpeed = _velocity.sqrMagnitude;
            _rotation = _rigidbody.rotation;
            // _noseRotation = _noseRotation;
            _state = DecideOnState();

            if (_isDrive)
            {
                // 状態に合わせた駆動処理
                switch (_state)
                {
                    case DragonDriverStateType.Walking: Walking(); break;
                    case DragonDriverStateType.Hovering: Hovering(); break;
                    case DragonDriverStateType.Flight: Flight(); break;
                    case DragonDriverStateType.Brakes: Brakes(); break;
                    case DragonDriverStateType.Overdrive: Overdrive(); break;
                }

                // 計算結果を出力
                _rigidbody.drag = _drag;
                _rigidbody.rotation = _rotation;
                _rigidbody.velocity = _velocity;
            }
            else
            {
                _noseRotation = _rotation;
            }

            // 同期
            sync_velocity = _rigidbody.velocity;
            sync_angularVelocity = _rigidbody.angularVelocity;
        }

        /******************************
         入力操作受付
         ******************************/
        public void Summon()
        {
            if (Networking.IsOwner(this.gameObject))
            {
                _objectSync.Respawn();
            }
        }

        public void _InputAccelerate(Vector3 acc)
        {
            _throttle = Vector3.ClampMagnitude(acc, 1.0f);
        }

        public void _InputRotate(Vector3 rot)
        {
            _elevator = Mathf.Clamp(rot.x, -1.0f, 1.0f);
            _ladder = Mathf.Clamp(rot.y, -1.0f, 1.0f);
            _aileron = Mathf.Clamp(rot.z, -1.0f, 1.0f);
        }

        public void _InputDirectRotate(Vector3 rot)
        {
            _directRotation = rot;
        }

        public void _InputGazeRotate(Quaternion rot)
        {
            _gazeRotation = rot;
        }

        public void _InputEmergencyBrakes(bool value)
        {
            _isBrakes = value;
        }

        public void _InputOverdrive(bool value)
        {
            _isOverdrive = value;
        }

        public void _InputJump()
        {
            if (_isGrounded)
            {
                _rigidbody.AddForce(_jumpImpulse * Vector3.up, ForceMode.VelocityChange);
            }
            else
            {
                _isWalking = false;
            }
        }

        /******************************
         状態別の処理
         ******************************/
        private DragonDriverStateType DecideOnState()
        {
            if (_isGrounded) { _isWalking = true; }

            if (_isBrakes)
            {
                _drag = _brakePower;
                _objectSync.SetGravity(false);
                return DragonDriverStateType.Brakes;
            }

            if (_isOverdrive && ((int)_enabledState & (int)DragonDriverEnabledStateSelect.Flight) > 0)
            {
                _drag = _defaultDrag;
                _objectSync.SetGravity(false);
                return DragonDriverStateType.Overdrive;
            }

            if (_isWalking && (((int)_enabledState & (int)DragonDriverEnabledStateSelect.Landing) > 0))
            {
                _drag = SetDrag(_sqrSpeed);
                _objectSync.SetGravity(true);
                return DragonDriverStateType.Walking;
            }

            if ((_sqrSpeed > _hoveringSpeedThreshold * _hoveringSpeedThreshold) &&
                ((int)_enabledState & (int)DragonDriverEnabledStateSelect.Flight) > 0)
            {
                _drag = SetDrag(_sqrSpeed);
                _objectSync.SetGravity(false);
                if (_gazeRotation != Quaternion.identity) { return DragonDriverStateType.Hovering; }
                return DragonDriverStateType.Flight;
            }

            if (((int)_enabledState & (int)DragonDriverEnabledStateSelect.Hovering) > 0)
            {
                _drag = SetDrag(_sqrSpeed);
                _objectSync.SetGravity(false);
                return DragonDriverStateType.Hovering;
            }

            _drag = SetDrag(_sqrSpeed);
            _objectSync.SetGravity(true);
            return default;
        }

        private void Walking()
        {
            // 自動バランサー制御
            var horizontalForward = Vector3.ProjectOnPlane(_rotation * Vector3.forward, Vector3.up);
            var horizontalRotation = Quaternion.LookRotation(horizontalForward);
            _rotation = Quaternion.RotateTowards(_rotation, horizontalRotation, Time.deltaTime * _landingSpeed);

            // 前後判定
            var noseDirection = _noseRotation * Vector3.forward;
            var velocityFront = Vector3.Project(_velocity, noseDirection);
            var velocitySide = Vector3.ProjectOnPlane(_velocity, noseDirection);
            var sign = (Vector3.Dot(noseDirection, velocityFront) < 0.0f) ? -1.0f : 1.0f;

            // 入力値計算
            var yaw = (_directRotation.y != 0.0f) ? _directRotation.y : Time.deltaTime * _noseRotateSpeed * _ladder;

            // 入力を反映
            var relativeRotation = _gazeRotation;
            if (_gazeRotation == Quaternion.identity)
            {
                relativeRotation = Quaternion.Inverse(_rotation) * _noseRotation;
                relativeRotation = Quaternion.AngleAxis(yaw, Vector3.up) * relativeRotation;
            }

            // 進行方向の軸制限
            var relativeDirection = relativeRotation * Vector3.forward;
            if (_isGrounded && _groundInfo.collider)
            {
                horizontalRotation = Quaternion.Inverse(_rotation) * Quaternion.FromToRotation(_rotation * Vector3.up, _groundInfo.normal) * _rotation;
                var groundForward = Vector3.ProjectOnPlane(relativeDirection, horizontalRotation * Vector3.up);
                var groundRotation = Quaternion.FromToRotation(relativeDirection, groundForward) * relativeRotation;
                relativeRotation = Quaternion.RotateTowards(relativeRotation, groundRotation, Time.deltaTime * _landingSpeed);

                if (_ladder == 0.0f)
                {
                    if (Quaternion.Angle(relativeRotation, horizontalRotation) < _centerSnapTolerance)
                    {
                        relativeRotation = horizontalRotation;
                    }
                }
            }

            // 可動範囲制限
            relativeRotation = ClampNoseRotation(relativeRotation);
            noseDirection = _rotation * relativeRotation * Vector3.forward;
            _noseRotation = Quaternion.LookRotation(noseDirection);

            // 本体を旋回
            var forward = _rotation * Vector3.forward;
            var up = _rotation * Vector3.up;
            relativeDirection = Vector3.ProjectOnPlane(noseDirection, up);
            var turnAngle = sign * Vector3.SignedAngle(forward, relativeDirection, up);
            var turn = Quaternion.AngleAxis(Time.deltaTime * Mathf.Clamp01(_sqrSpeed) * turnAngle, up);
            _rotation = turn * _rotation;
            _noseRotation = turn * _noseRotation;

            // 速度の再計算
            _velocity = turn * _velocity;

            if (CheckStill(_targetVelocity.sqrMagnitude))
            {
                _targetVelocity = Vector3.zero;
                return;
            }
            var velocityForward = Vector3.Project(Quaternion.Inverse(_noseRotation) * _velocity, _targetVelocity);
            _targetVelocity = CalculateTargetVelocity(_targetVelocity, _maxWalkSpeed);
            _velocity += Time.deltaTime * _acceleration * (_noseRotation * (_targetVelocity - velocityForward));
        }

        private void Hovering()
        {
            // 自動バランサー制御
            var forward = _rotation * Vector3.forward;
            var noseDirection = _noseRotation * Vector3.forward;
            var horizontalForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            var horizontalRotation = Quaternion.LookRotation(horizontalForward);
            var targetRotation = Quaternion.RotateTowards(_rotation, horizontalRotation, Time.deltaTime * _stateShiftSpeed);
            var baranceRotation = Quaternion.Inverse(_rotation) * Quaternion.Inverse(Quaternion.FromToRotation(forward, targetRotation * Vector3.forward)) * targetRotation;
            var baranceRoll = Vector3.SignedAngle(Vector3.up, baranceRotation * Vector3.up, Vector3.forward);
            _rotation = targetRotation;
            _noseRotation = Quaternion.AngleAxis(baranceRoll, Vector3.up) * _noseRotation;

            // 前後判定
            var velocityFront = Vector3.Project(_velocity, noseDirection);
            var velocitySide = Vector3.ProjectOnPlane(_velocity, noseDirection);
            var sign = (Vector3.Dot(noseDirection, velocityFront) < 0.0f) ? -1.0f : 1.0f;

            // 入力値計算
            var pitch = (_directRotation.x != 0.0f) ? _directRotation.x : Time.deltaTime * _noseRotateSpeed * _elevator;
            var yaw = (_directRotation.y != 0.0f) ? _directRotation.y : Time.deltaTime * _noseRotateSpeed * _ladder;

            // 入力を反映
            var relativeRotation = _gazeRotation;
            if (_gazeRotation == Quaternion.identity)
            {
                relativeRotation = Quaternion.Inverse(_rotation) * _noseRotation;
                relativeRotation = Quaternion.AngleAxis(pitch, Vector3.right) * relativeRotation;
                relativeRotation = Quaternion.AngleAxis(yaw, Vector3.up) * relativeRotation;
            }

            // 進行方向の軸制限
            if (_elevator == 0.0f && _ladder == 0.0f)
            {
                if (Quaternion.Angle(Quaternion.identity, relativeRotation) < _centerSnapTolerance)
                {
                    relativeRotation = Quaternion.identity;
                }
            }

            // 可動範囲制限
            relativeRotation = ClampNoseRotation(relativeRotation);
            noseDirection = _rotation * relativeRotation * Vector3.forward;
            _noseRotation = Quaternion.LookRotation(noseDirection);

            // 本体を旋回
            var horizontalNoseDirection = Vector3.ProjectOnPlane(noseDirection, Vector3.up);
            var turnAngle = sign * Vector3.SignedAngle(horizontalForward, horizontalNoseDirection, Vector3.up);
            var turn = Quaternion.AngleAxis(Time.deltaTime * Mathf.Clamp01(_sqrSpeed) * turnAngle, Vector3.up);
            _rotation = turn * _rotation;
            _noseRotation = turn * _noseRotation;

            // 速度の再計算
            if (CheckStill(_targetVelocity.sqrMagnitude))
            {
                _targetVelocity = Vector3.zero;
                return;
            }
            _targetVelocity = CalculateTargetVelocity(_targetVelocity, _maxSpeed);
            _velocity += Time.deltaTime * _acceleration * (_noseRotation * _targetVelocity - _velocity);
        }

        private void Flight()
        {
            // 自動バランサー制御
            var noseDirection = _noseRotation * Vector3.forward;
            var targetRotation = Quaternion.FromToRotation(_rotation * Vector3.forward, noseDirection) * _rotation;
            targetRotation = Quaternion.RotateTowards(_rotation, targetRotation, Time.deltaTime * _stateShiftSpeed);

            var horizontalDirection = Vector3.ProjectOnPlane(_rotation * Vector3.forward, Vector3.up);
            var horizontalTargetDirection = Vector3.ProjectOnPlane(targetRotation * Vector3.forward, Vector3.up);
            var baranceRoll = Vector3.SignedAngle(horizontalTargetDirection, horizontalDirection, Vector3.up);
            _rotation = targetRotation * Quaternion.AngleAxis(baranceRoll, Vector3.forward);

            // 前後判定
            var velocityFront = Vector3.Project(_velocity, noseDirection);
            var velocitySide = Vector3.ProjectOnPlane(_velocity, noseDirection);
            var sign = (Vector3.Dot(noseDirection, velocityFront) < 0.0f) ? -1.0f : 1.0f;

            // 入力値計算
            if (_inertialInputDuration > 0.0f)
            {
                _inertialPitch = Mathf.MoveTowards(_inertialPitch, _elevator, Time.deltaTime / _inertialInputDuration);
                _inertialRoll = Mathf.MoveTowards(_inertialRoll, _aileron, Time.deltaTime / _inertialInputDuration);
            }
            else
            {
                _inertialPitch = _elevator;
                _inertialRoll = _aileron;
            }
            var pitch = Time.deltaTime * _updownSpeed * _inertialPitch;
            var roll = Time.deltaTime * _rollSpeed * _inertialRoll;

            // 入力を反映
            var calculateRotation = Quaternion.AngleAxis(roll, Vector3.forward);
            calculateRotation = _rotation * calculateRotation * Quaternion.Inverse(_rotation);
            _rotation = calculateRotation * _rotation;
            _noseRotation = calculateRotation * _noseRotation;

            var up = (Vector3.Dot(_rotation * Vector3.up, Vector3.up) < 0.0f) ? -1.0f : 1.0f;
            horizontalDirection = up * Vector3.Cross(Vector3.up, _rotation * Vector3.forward);
            calculateRotation = Quaternion.AngleAxis(pitch, horizontalDirection);
            _rotation = calculateRotation * _rotation;
            _noseRotation = calculateRotation * _noseRotation;

            var turn = Vector3.Dot(Vector3.up, _rotation * Vector3.left);
            turn *= Time.deltaTime * _rollSpeed * _rollToTurnRatio;
            // 本体を旋回
            calculateRotation = Quaternion.AngleAxis(sign * turn, Vector3.up);
            _rotation = calculateRotation * _rotation;
            _noseRotation = calculateRotation * _noseRotation;

            // 進行方向確定
            noseDirection = _noseRotation * Vector3.forward;
            _noseRotation = Quaternion.LookRotation(noseDirection);

            // 速度の再計算
            if (CheckStill(_targetVelocity.sqrMagnitude))
            {
                _targetVelocity = Vector3.zero;
                return;
            }
            _targetVelocity = CalculateTargetVelocity(_targetVelocity, _maxSpeed);
            _velocity += Time.deltaTime * _acceleration * (_noseRotation * _targetVelocity - _velocity);
        }

        private void Brakes()
        {
            // 自動バランサー制御            
            var forward = _rotation * Vector3.forward;
            var noseDirection = _noseRotation * Vector3.forward;
            var horizontalForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            if (_sqrSpeed < _stillSpeedThreshold * _stillSpeedThreshold)
            {
                var horizontalRotation = Quaternion.LookRotation(horizontalForward);
                var targetRotation = Quaternion.RotateTowards(_rotation, horizontalRotation, Time.deltaTime * _stateShiftSpeed);
                var baranceRotation = Quaternion.Inverse(_rotation) * Quaternion.Inverse(Quaternion.FromToRotation(forward, targetRotation * Vector3.forward)) * targetRotation;
                var baranceRoll = Vector3.SignedAngle(Vector3.up, baranceRotation * Vector3.up, Vector3.forward);
                _rotation = targetRotation;
                _noseRotation = Quaternion.AngleAxis(baranceRoll, Vector3.up) * _noseRotation;
            }
            else
            {
                var targetRotation = Quaternion.FromToRotation(forward, noseDirection) * _rotation;
                targetRotation = Quaternion.RotateTowards(_rotation, targetRotation, Time.deltaTime * _stateShiftSpeed);
                var horizontalTargetRotation = Vector3.ProjectOnPlane(targetRotation * Vector3.forward, Vector3.up);
                var baranceRoll = Vector3.SignedAngle(horizontalTargetRotation, horizontalForward, Vector3.up);

                _rotation = targetRotation * Quaternion.AngleAxis(baranceRoll, Vector3.forward);
            }

            // 可動範囲制限
            var relativeRotation = ClampNoseRotation(Quaternion.Inverse(_rotation) * _noseRotation);
            noseDirection = _rotation * relativeRotation * Vector3.forward;
            _noseRotation = Quaternion.LookRotation(noseDirection);

            // 速度の再計算
            var velocityForward = Vector3.Project(Quaternion.Inverse(_noseRotation) * _velocity, _targetVelocity);
            _targetVelocity = Vector3.ClampMagnitude(velocityForward, _maxSpeed);
            // 本体速度は物理減速に任せる
            //_velocity += Time.deltaTime * _acceleration * (_noseRotation * _targetVelocity - _velocity);
        }

        private void Overdrive() { }

        /******************************
         その他内部処理
         ******************************/
        private bool CheckGrounded()
        {
            var origin = _rigidbody.position + _rigidbody.rotation * _colliderCenter;
            if (Physics.SphereCast
                (
                    origin,
                    _groundCheckRadius,
                    Vector3.down,
                    out _groundInfo,
                    _groundCheckRange,
                    _groundLayer,
                    QueryTriggerInteraction.Ignore
                ))
            { return Vector3.Angle(Vector3.up, _groundInfo.normal) < _slopeLimit; }

            return false;
        }

        private float SetDrag(float sqrSpeed)
        {
            if (sqrSpeed < _stillSpeedThreshold * _stillSpeedThreshold)
            {
                if (_throttle == Vector3.zero)
                {
                    return _stillDrag;
                }
            }

            return _defaultDrag;
        }

        private Quaternion ClampNoseRotation(Quaternion relativeRotation)
        {
            var relativeDirection = relativeRotation * Vector3.forward;
            var AxisDirection = Vector3.ProjectOnPlane(relativeDirection, Vector3.right);
            var relativeAngle = Vector3.SignedAngle(Vector3.forward, AxisDirection, Vector3.right);
            if (relativeAngle < -_maxNosePitch)
            {
                relativeRotation = Quaternion.AngleAxis(-_maxNosePitch - relativeAngle, Vector3.right) * relativeRotation;
            }
            if (relativeAngle > _maxNosePitch)
            {
                relativeRotation = Quaternion.AngleAxis(_maxNosePitch - relativeAngle, Vector3.right) * relativeRotation;
            }
            relativeDirection = relativeRotation * Vector3.forward;
            AxisDirection = Vector3.ProjectOnPlane(relativeDirection, Vector3.up);
            relativeAngle = Vector3.SignedAngle(Vector3.forward, AxisDirection, Vector3.up);
            if (relativeAngle < -_maxNoseYaw)
            {
                relativeRotation = Quaternion.AngleAxis(-_maxNoseYaw - relativeAngle, Vector3.up) * relativeRotation;
            }
            if (relativeAngle > _maxNoseYaw)
            {
                relativeRotation = Quaternion.AngleAxis(_maxNoseYaw - relativeAngle, Vector3.up) * relativeRotation;
            }

            return relativeRotation;
        }

        private bool CheckStill(float sqrSpeed)
        {
            return (_throttle == Vector3.zero) &&
                   (sqrSpeed < _stillSpeedThreshold * _stillSpeedThreshold);
        }

        private Vector3 CalculateTargetVelocity(Vector3 targetVelocity, float maxSpeed)
        {
            // 入力値計算
            var thrust = Time.deltaTime * _acceleration * _throttle;

            var velocityForward = Vector3.Project(Quaternion.Inverse(_noseRotation) * _velocity, targetVelocity);
            var differenceVelocity = targetVelocity - velocityForward;
            if (differenceVelocity.sqrMagnitude > _accelerateLimit * _accelerateLimit)
            {
                targetVelocity = Vector3.ClampMagnitude(targetVelocity, _accelerateLimit) + velocityForward;
            }
            targetVelocity = Vector3.ClampMagnitude(targetVelocity + thrust, maxSpeed);

            return targetVelocity;
        }
    }
}
