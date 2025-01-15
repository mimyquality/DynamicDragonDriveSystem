/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
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

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Driver")]
    [RequireComponent(typeof(Rigidbody), typeof(VRCObjectSync), typeof(SphereCollider))]
    [DefaultExecutionOrder(0)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class DragonDriver : UdonSharpBehaviour
    {
        private const float VelocitySmoothingDuration = 0.05f;

        [Header("Speed Settings")]
        [SerializeField, Tooltip("m/s^2")]
        private float _acceleration = 5.0f;
        [SerializeField, Tooltip("m/s")]
        private float _maxSpeed = 56.0f;
        [SerializeField, Tooltip("m/s")]
        private float _maxWalkSpeed = 16.0f;
        [SerializeField, Tooltip("m/s")]
        private float _hoveringSpeedThreshold = 15.0f;
        [SerializeField, Tooltip("m/s")]
        private float _stillSpeedThreshold = 3.0f;
        [SerializeField, Min(0.0f)]
        private float _stillDrag = 1.0f;

        [Header("Rotate Settings")]
        [SerializeField, Tooltip("deg/s")]
        private float _updownSpeed = 45.0f;
        [SerializeField, Tooltip("deg/s")]
        private float _rollSpeed = 60.0f;
        [SerializeField, Range(0.0f, 2.0f)]
        private float _updownToTurnRatio = 1.0f;
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
        private float _stateShiftSpeed = 40.0f;
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

        // VRCObjectSyncが同期してくれない分
        [UdonSynced(UdonSyncMode.Linear)]
        private Vector3 sync_velocity;
        [UdonSynced(UdonSyncMode.Linear)]
        private Vector3 sync_angularVelocity;

        // コンポーネント
        private Rigidbody _rigidbody;
        private SphereCollider _collider;
        private VRCObjectSync _objectSync;

        // 計算用
        private bool _isOwner;
        private bool _isSleeping = true;
        private Vector3 _velocity, _targetVelocity;
        private Quaternion _rotation;
        private Vector3 _noseAngles, _targetNoseAngles;
        private Vector3 _baseNoseAngles;
        private float _drag, _defaultDrag, _sqrSpeed;
        private DragonDriverStateType _state;
        private bool _isWalking, _isGrounded, _isBrakes, _isOverdrive;
        private float _inertialRoll, _inertialPitch;
        private Vector3 _colliderCenter;
        private float _groundCheckRadius, _groundCheckRange;
        private RaycastHit _groundInfo = new RaycastHit();
        private Vector3 _currentVelocity;
        private Vector3 _currentAngularVelocity;
        private Vector3 _currentAcceleration;
        private Vector3 _currentAngularAcceleration;

        // Input
        private bool _isDrive;
        private Vector3 _throttle;
        private float _elevator, _ladder, _aileron;
        private Vector3 _directRotation;
        private bool _isAbsolute;

        // Actor渡し用
        public bool IsGrounded { get => _isGrounded; }
        public bool IsBrakes { get => _isBrakes; }
        public bool IsOverdrive { get => _isOverdrive; }
        public int State { get => (int)_state; }
        public Vector3 Velocity
        {
            get =>
                _isSleeping ? Vector3.zero :
                _isOwner ? _currentVelocity :
                sync_velocity;
        }
        public Vector3 AngularVelocity
        {
            get =>
                _isSleeping ? Vector3.zero :
                _isOwner ? _currentAngularVelocity :
                sync_angularVelocity;
        }
        public Vector3 NoseAngles { get => _noseAngles; }
        public Vector3 GroundNormal { get => _isGrounded ? _groundInfo.normal : Vector3.up; }

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
        public float RollToTurnRatio { get => _updownToTurnRatio; set => _updownToTurnRatio = Mathf.Clamp(value, 0.0f, 2.0f); }
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

            var scale = transform.lossyScale;
            _colliderCenter = Vector3.Scale(_collider.center, scale);
            var radius = Mathf.Max(scale.x, scale.y, scale.z) * _collider.radius;
            _groundCheckRadius = 0.9f * radius;
            _groundCheckRange = 0.2f * radius;

            _isOwner = Networking.IsOwner(this.gameObject);

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

            if (!_isOwner) { return; }
            // 以下Ownerの処理

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
                if (_directRotation == Vector3.zero) _baseNoseAngles = _targetNoseAngles;
                _rigidbody.drag = _drag;
                _rigidbody.rotation = _rotation;
                _rigidbody.velocity = _velocity;
            }
            else
            {
                _noseAngles = Vector3.zero;
            }

            // 同期
            sync_velocity = _rigidbody.velocity;
            sync_angularVelocity = _rigidbody.angularVelocity;

            // なんかハンチングするのでOwnerはスムージング処理
            _currentVelocity = Vector3.SmoothDamp(_currentVelocity, sync_velocity, ref _currentAcceleration, VelocitySmoothingDuration);
            _currentAngularVelocity = Vector3.SmoothDamp(_currentAngularVelocity, sync_angularVelocity, ref _currentAngularAcceleration, VelocitySmoothingDuration);
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            _isOwner = player.isLocal;
        }

        /******************************
         入力操作受付
         ******************************/
        public void Respawn()
        {
            if (Networking.IsOwner(this.gameObject))
            {
                _objectSync.Respawn();
            }
        }

        public void TeleportTo(Transform target)
        {
            if (Networking.IsOwner(this.gameObject))
            {
                _objectSync.TeleportTo(target);
            }
        }

        public void _InputAccelerate(Vector3 acceleration)
        {
            _throttle = Vector3.ClampMagnitude(acceleration, 1.0f);
        }

        public void _InputRotate(Vector3 angles)
        {
            _elevator = Mathf.Clamp(angles.x, -1.0f, 1.0f);
            _ladder = Mathf.Clamp(angles.y, -1.0f, 1.0f);
            _aileron = Mathf.Clamp(angles.z, -1.0f, 1.0f);
        }

        public void _InputDirectRotate(Vector3 angles)
        {
            _InputDirectRotate(angles, false);
        }

        public void _InputDirectRotate(Vector3 angles, bool isAbsolute)
        {
            _directRotation = angles;
            _isAbsolute = isAbsolute;
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
                // 垂直ジャンプだとなんか後ろに飛ぶので
                var jumpDirection = Quaternion.LookRotation(Vector3.up, _rigidbody.rotation * Vector3.forward) * new Vector3(0.0f, 0.001f, 1.0f);
                _rigidbody.AddForce(_jumpImpulse * jumpDirection, ForceMode.VelocityChange);
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
                if (_isAbsolute) { return DragonDriverStateType.Hovering; }
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
            // 前後判定
            var noseDirection = _rotation * Quaternion.Euler(_noseAngles) * Vector3.forward;
            var sign = Mathf.Sign(Vector3.Dot(_velocity, noseDirection));

            // 入力値計算
            if (_isAbsolute)
            {
                _targetNoseAngles.y = _directRotation.y;
                if (Mathf.Abs(_targetNoseAngles.y) < _centerSnapTolerance)
                {
                    _targetNoseAngles.y = 0.0f;
                }
            }
            else if (_directRotation != Vector3.zero)
            {
                _targetNoseAngles.y = _baseNoseAngles.y + _directRotation.y;
            }
            else
            {
                _targetNoseAngles.y += Time.deltaTime * _noseRotateSpeed * _ladder;
                if (_ladder == 0.0f && Mathf.Abs(_targetNoseAngles.y) < _centerSnapTolerance)
                {
                    _targetNoseAngles.y = 0.0f;
                }
            }
            _targetNoseAngles.y = Mathf.Clamp(_targetNoseAngles.y, -_maxNoseYaw, _maxNoseYaw);
            var yaw = Mathf.MoveTowards(_noseAngles.y, _targetNoseAngles.y, Time.deltaTime * _noseRotateSpeed);

            // 自動バランサー制御
            var horizontalForward = Vector3.ProjectOnPlane(_rotation * Vector3.forward, Vector3.up);
            var horizontalRotation = Quaternion.LookRotation(horizontalForward);
            var fixedRotation = Quaternion.RotateTowards(_rotation, horizontalRotation, Time.deltaTime * _landingSpeed);

            // 本体を旋回
            var turn = Quaternion.AngleAxis(Time.deltaTime * Mathf.Clamp01(_sqrSpeed) * sign * yaw, Vector3.up);
            fixedRotation = turn * fixedRotation;

            // 進行方向の軸制限
            _targetNoseAngles.x = 0.0f;
            var pitch = 0.0f;
            var noseRotation = Quaternion.Euler(pitch, yaw, 0.0f) * fixedRotation;
            if (_isGrounded && _groundInfo.collider)
            {
                noseDirection = noseRotation * Vector3.forward;
                var horizontalNoseDirection = Vector3.ProjectOnPlane(noseDirection, _groundInfo.normal);
                pitch = Vector3.SignedAngle(noseDirection, horizontalNoseDirection, noseRotation * Vector3.right);
                pitch = Mathf.Clamp(pitch, -_maxNosePitch, _maxNosePitch);
            }

            // 回転の確定と再補正
            _velocity = Quaternion.Inverse(_rotation) * fixedRotation * _velocity;
            _rotation = fixedRotation;
            _noseAngles = new Vector3(pitch, yaw, 0.0f);

            // 静止判定
            if (CheckStill(_targetVelocity.sqrMagnitude))
            {
                _targetVelocity = Vector3.zero;
                return;
            }

            // 速度の再計算
            noseRotation = fixedRotation * Quaternion.Euler(_noseAngles);
            var relativeVelocity = Quaternion.Inverse(noseRotation) * _velocity;
            var velocityForward = Vector3.Project(relativeVelocity, _targetVelocity);
            _targetVelocity = CalculateTargetVelocity(_targetVelocity, relativeVelocity, _maxWalkSpeed);
            _velocity += Time.deltaTime * _acceleration * (noseRotation * (_targetVelocity - velocityForward));
        }

        private void Hovering()
        {
            // 前後判定
            var noseDirection = _rotation * Quaternion.Euler(_noseAngles) * Vector3.forward;
            var sign = Mathf.Sign(Vector3.Dot(_velocity, noseDirection));

            // 入力値計算
            if (_isAbsolute)
            {
                _targetNoseAngles.x = _directRotation.x;
                _targetNoseAngles.y = _directRotation.y;
                _targetNoseAngles.z = 0.0f;
                if (Vector3.Angle(Vector3.forward, Quaternion.Euler(_targetNoseAngles) * Vector3.forward) < _centerSnapTolerance)
                {
                    _targetNoseAngles.x = 0.0f;
                    _targetNoseAngles.y = 0.0f;
                }
            }
            else if (_directRotation != Vector3.zero)
            {
                _targetNoseAngles.x = _baseNoseAngles.x + _directRotation.x;
                _targetNoseAngles.y = _baseNoseAngles.y + _directRotation.y;
            }
            else
            {
                _targetNoseAngles.x += Time.deltaTime * _noseRotateSpeed * _elevator;
                _targetNoseAngles.y += Time.deltaTime * _noseRotateSpeed * _ladder;
                _targetNoseAngles.z = 0.0f;
                if (_elevator == 0.0f && _ladder == 0.0f
                && (Vector3.Angle(Vector3.forward, Quaternion.Euler(_targetNoseAngles) * Vector3.forward) < _centerSnapTolerance))
                {
                    _targetNoseAngles.x = 0.0f;
                    _targetNoseAngles.y = 0.0f;
                }
            }
            _targetNoseAngles.x = Mathf.Clamp(_targetNoseAngles.x, -_maxNosePitch, _maxNosePitch);
            _targetNoseAngles.y = Mathf.Clamp(_targetNoseAngles.y, -_maxNoseYaw, _maxNoseYaw);
            var pitch = Mathf.MoveTowards(_noseAngles.x, _targetNoseAngles.x, Time.deltaTime * _noseRotateSpeed);
            var yaw = Mathf.MoveTowards(_noseAngles.y, _targetNoseAngles.y, Time.deltaTime * _noseRotateSpeed);

            // 自動バランサー制御
            var horizontalForward = Vector3.ProjectOnPlane(_rotation * Vector3.forward, Vector3.up);
            var horizontalRotation = Quaternion.LookRotation(horizontalForward);
            var fixedRotation = Quaternion.RotateTowards(_rotation, horizontalRotation, Time.deltaTime * _stateShiftSpeed);

            // 本体を旋回
            var turn = Quaternion.AngleAxis(Time.deltaTime * Mathf.Clamp01(_sqrSpeed) * sign * yaw, Vector3.up);
            fixedRotation = turn * fixedRotation;

            // 回転の確定と再補正
            _velocity = Quaternion.Inverse(_rotation) * fixedRotation * _velocity;
            _rotation = fixedRotation;
            _noseAngles = new Vector3(pitch, yaw, 0.0f);

            // 静止判定
            if (CheckStill(_targetVelocity.sqrMagnitude))
            {
                _targetVelocity = Vector3.zero;
                return;
            }

            // 速度の再計算
            var noseRotation = fixedRotation * Quaternion.Euler(_noseAngles);
            var relativeVelocity = Quaternion.Inverse(noseRotation) * _velocity;
            _targetVelocity = CalculateTargetVelocity(_targetVelocity, relativeVelocity, _maxSpeed);
            _velocity += Time.deltaTime * _acceleration * (noseRotation * _targetVelocity - _velocity);
        }

        private void Flight()
        {
            // 前後判定
            var noseDirection = _rotation * Quaternion.Euler(_noseAngles) * Vector3.forward;
            var sign = Mathf.Sign(Vector3.Dot(_velocity, noseDirection));

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

            // 可動範囲制限
            _targetNoseAngles = Vector3.MoveTowards(_targetNoseAngles, Vector3.zero, Time.deltaTime * _stateShiftSpeed);
            _noseAngles = Vector3.MoveTowards(_noseAngles, _targetNoseAngles, Time.deltaTime * _stateShiftSpeed);

            // 自動バランサー制御
            var fixedRotation = _rotation;

            //入力を本体に反映
            var calculateRotation = Quaternion.AngleAxis(roll, Vector3.forward);
            fixedRotation = fixedRotation * calculateRotation;

            var fixedForward = fixedRotation * Vector3.forward;
            var fixedUp = fixedRotation * Vector3.up;
            var upAxis = (Vector3.Dot(fixedUp, Vector3.up) < 0.0f) ? Vector3.down : Vector3.up;
            var horizontalRotation = Quaternion.LookRotation(fixedForward, upAxis);
            var pitchCorrection = Mathf.Abs(Vector3.Angle(fixedUp, Vector3.up) / 90.0f - 1.0f);
            calculateRotation = Quaternion.AngleAxis(pitchCorrection * pitch, horizontalRotation * Vector3.right);
            fixedRotation = calculateRotation * fixedRotation;

            pitchCorrection = Mathf.Abs(Vector3.Angle(fixedForward, Vector3.up) / 90.0f - 1.0f);
            calculateRotation = Quaternion.AngleAxis(pitchCorrection * pitch, Vector3.right);
            fixedRotation = fixedRotation * calculateRotation;

            // 本体を横旋回
            var turn = Vector3.Dot(fixedRotation * Vector3.left, Vector3.up);
            turn *= Time.deltaTime * sign * _updownToTurnRatio * _updownSpeed;
            calculateRotation = Quaternion.AngleAxis(turn, Vector3.up);
            fixedRotation = calculateRotation * fixedRotation;

            // 回転の確定と再補正
            _velocity = Quaternion.Inverse(_rotation) * fixedRotation * _velocity;
            _rotation = fixedRotation;

            // 静止判定
            if (CheckStill(_targetVelocity.sqrMagnitude))
            {
                _targetVelocity = Vector3.zero;
                return;
            }

            // 速度の再計算
            var noseRotation = fixedRotation * Quaternion.Euler(_noseAngles);
            var relativeVelocity = Quaternion.Inverse(noseRotation) * _velocity;
            _targetVelocity = CalculateTargetVelocity(_targetVelocity, relativeVelocity, _maxSpeed);
            _velocity += Time.deltaTime * _acceleration * (noseRotation * _targetVelocity - _velocity);
        }

        private void Brakes()
        {
            var fixedRotation = _rotation;

            // 自動バランサー制御
            if (_sqrSpeed < _stillSpeedThreshold * _stillSpeedThreshold)
            {
                var horizontalForward = Vector3.ProjectOnPlane(_rotation * Vector3.forward, Vector3.up);
                var horizontalRotation = Quaternion.LookRotation(horizontalForward);
                fixedRotation = Quaternion.RotateTowards(_rotation, horizontalRotation, Time.deltaTime * _stateShiftSpeed);
            }
            else
            {
                var BalancedRotation = Quaternion.LookRotation(_rotation * Vector3.forward);
                fixedRotation = Quaternion.RotateTowards(_rotation, BalancedRotation, Time.deltaTime * _stateShiftSpeed);
            }

            // 回転の確定と再補正
            _velocity = Quaternion.Inverse(_rotation) * fixedRotation * _velocity;
            _rotation = fixedRotation;

            // 速度の再計算
            var noseRotation = fixedRotation * Quaternion.Euler(_noseAngles);
            var relativeVelocity = Quaternion.Inverse(noseRotation) * _velocity;
            var velocityForward = Vector3.Project(relativeVelocity, _targetVelocity);
            _targetVelocity = Vector3.ClampMagnitude(velocityForward, _maxSpeed);
            // 本体速度は物理減速に任せる
            //_velocity += Time.deltaTime * _acceleration * (noseRotation * _targetVelocity - _velocity);
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

        private bool CheckStill(float sqrSpeed)
        {
            return (_throttle == Vector3.zero) &&
                   (sqrSpeed < _stillSpeedThreshold * _stillSpeedThreshold);
        }

        private Vector3 CalculateTargetVelocity(Vector3 targetVelocity, Vector3 currentVelocity, float maxSpeed)
        {
            // 入力値計算
            var thrust = Time.deltaTime * _acceleration * _throttle;

            //var limitVelocity = Vector3.Project(currentVelocity, targetVelocity);
            var limitVelocity = currentVelocity;
            if (Vector3.Dot(targetVelocity, limitVelocity) > 0.0f)
            {
                // ToDo:ここの係数見直し
                var limitSpeed = (1.0f + Time.deltaTime * (_drag + _acceleration)) * limitVelocity.magnitude + _stillSpeedThreshold;
                if (targetVelocity.sqrMagnitude > limitSpeed * limitSpeed)
                {
                    targetVelocity = Vector3.ClampMagnitude(targetVelocity, limitSpeed);
                }
            }
            targetVelocity = Vector3.ClampMagnitude(targetVelocity + thrust, maxSpeed);

            return targetVelocity;
        }
    }
}
