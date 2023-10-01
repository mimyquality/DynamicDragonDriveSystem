
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
    public enum DragonDrivingStateType
    {
        Walking,
        Hovering,
        Flight,
        Brakes,
        Overdrive
    }

    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider), typeof(VRCObjectSync))]
    public class DragonDriver : UdonSharpBehaviour
    {
        private const float ConvertKMPHtoMPS = 5 / 18f;

        [Header("Speed settings")]
        [Tooltip("m/s^2"), SerializeField]
        private float _acceleration = 5.0f;
        [Tooltip("km/h"), SerializeField]
        private float _maxSpeed = 200.0f;
        [Tooltip("km/h"), SerializeField]
        private float _maxWalkSpeed = 60.0f;
        [Tooltip("km/h"), SerializeField]
        private float _hoveringSpeedThreshold = 60.0f;
        [Tooltip("km/h"), SerializeField]
        private float _stillSpeedThreshold = 5.0f;
        [Min(0.0f), SerializeField]
        private float _stillDrag = 1.0f;

        [Header("Rotate settings")]
        [Tooltip("deg/s"), SerializeField]
        private float _updownSpeed = 45.0f;
        [Tooltip("deg/s"), SerializeField]
        private float _rollSpeed = 45.0f;
        [Range(0.0f, 2.0f), SerializeField]
        private float _rollToTurnRatio = 1.0f;
        [Tooltip("sec"), Min(0.0f), SerializeField]
        private float _inertialInputDuration = 0.3f;
        [Tooltip("deg/s"), SerializeField]
        private float _noseRotateSpeed = 90.0f;
        [Tooltip("degree"), Range(0.0f, 89.0f), SerializeField]
        private float _maxNosePitch = 89.0f;
        [Tooltip("degree"), Range(0.0f, 89.0f), SerializeField]
        private float _maxNoseYaw = 45.0f;
        [Tooltip("degree"), Range(0.0f, 89.0f), SerializeField]
        private float _centerSnapTolerance = 5.0f;
        [Tooltip("deg/s"), SerializeField]
        private float _stateShiftSpeed = 20.0f;
        [Tooltip("deg/s"), SerializeField]
        private float _landingSpeed = 30.0f;

        [Header("Others")]
        [Tooltip("m/s"), Min(0.0f), SerializeField]
        private float _accelerateLimit = 18.0f * 5 / 18;
        [Tooltip("m/s"), SerializeField]
        private float _jumpSpeed = 30.0f * 5 / 18;
        [Min(0.0f), SerializeField]
        private float _brakePower = 2.0f;
        [SerializeField]
        private LayerMask _groundLayer = 0b00000000000000111000100100010111;
        [Tooltip("degree"), Range(0.0f, 89.9f), SerializeField]
        private float _slopeLimit = 45.0f;

        // 単位変換用
        [FieldChangeCallback(nameof(MaxSpeed))]
        private float _maxSpeedConverted;
        public float MaxSpeed
        {
            get => _maxSpeedConverted;
            set => _maxSpeedConverted = value;
        }
        [FieldChangeCallback(nameof(MaxWalkSpeed))]
        private float _maxWalkSpeedConverted;
        public float MaxWalkSpeed
        {
            get => _maxWalkSpeedConverted;
            set => _maxWalkSpeedConverted = value;
        }
        [FieldChangeCallback(nameof(HoveringSpeedThreshold))]
        private float _hoveringSpeedThresholdConverted;
        public float HoveringSpeedThreshold
        {
            get => _hoveringSpeedThresholdConverted;
            set => _hoveringSpeedThresholdConverted = value;
        }
        [FieldChangeCallback(nameof(StillSpeedThreshold))]
        private float _stillSpeedThresholdConverted;
        public float StillSpeedThreshold
        {
            get => _stillSpeedThresholdConverted;
            set => _stillSpeedThresholdConverted = value;
        }

        // Actor渡し用
        public bool IsMount { get => _isMount; set => _isMount = value; }
        public int State { get => (int)_state; }
        public bool IsGrounded { get => _isGrounded; }
        public bool IsBrakes { get => _isBrakes; }
        public bool IsOverdrive { get => _isOverdrive; }
        public float TargetSpeed { get => _targetVelocity.magnitude; }
        public Quaternion NoseRotation { get => _noseRotation; }

        // コンポーネント
        private Rigidbody _rigidbody;
        private SphereCollider _collider;
        private VRCObjectSync _objectSync;

        // 計算用
        private Vector3 _velocity, _targetVelocity;
        private Quaternion _rotation, _noseRotation;
        private float _drag, _defaultDrag, _sqrSpeed;
        private DragonDrivingStateType _state;
        private bool _isWalking, _isGrounded, _isBrakes, _isOverdrive;
        private float _inertialRoll, _inertialPitch;
        private float _groundCheckRadius, _groundCheckRange;
        private RaycastHit _groundInfo = new RaycastHit();

        // Input
        private Vector3 _throttle;
        private float _elevator, _ladder, _aileron;
        [FieldChangeCallback(nameof(IsMount))]
        private bool _isMount;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();
            _objectSync = GetComponent<VRCObjectSync>();

            _rigidbody.freezeRotation = true;
            _rigidbody.maxDepenetrationVelocity = _maxSpeedConverted;
            _objectSync.SetKinematic(false);
            _objectSync.SetGravity(false);

            MaxSpeed = _maxSpeed * ConvertKMPHtoMPS;
            MaxWalkSpeed = _maxWalkSpeed * ConvertKMPHtoMPS;
            HoveringSpeedThreshold = _hoveringSpeedThreshold * ConvertKMPHtoMPS;
            StillSpeedThreshold = _stillSpeedThreshold * ConvertKMPHtoMPS;

            _defaultDrag = _rigidbody.drag;
            _groundCheckRadius = _collider.radius * 0.9f;
            _groundCheckRange = 2 * (_collider.radius - _groundCheckRadius);

            _initialized = true;
        }
        private void OnEnable()
        {
            Initialize();

            _noseRotation = _rigidbody.rotation;
        }

        private void OnDisable()
        {
            _InputAccelerate(Vector3.zero);
            _InputRotate(Vector3.zero);
            _InputEmergencyBrakes(false);
            _InputOverdrive(false);

            _noseRotation = _rigidbody.rotation;

            _targetVelocity = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            // 現状を算出
            _velocity = _rigidbody.velocity;
            _sqrSpeed = _velocity.sqrMagnitude;
            _rotation = _rigidbody.rotation;
            // _noseRotation = _noseRotation;
            if (_isGrounded = CheckGrounded()) { _isWalking = true; }

            if (_isBrakes)
            {
                _state = DragonDrivingStateType.Brakes;
            }
            else if (_isWalking)
            {
                _state = DragonDrivingStateType.Walking;
            }
            else if (_isOverdrive)
            {
                _state = DragonDrivingStateType.Overdrive;
            }
            else if (_sqrSpeed > _hoveringSpeedThresholdConverted * _hoveringSpeedThresholdConverted)
            {
                _state = DragonDrivingStateType.Flight;
            }
            else
            {
                _state = DragonDrivingStateType.Hovering;
            }

            // 状態に合わせたRigidbodyの計算
            switch (_state)
            {
                case DragonDrivingStateType.Walking: Walking(); break;
                case DragonDrivingStateType.Hovering: Hovering(); break;
                case DragonDrivingStateType.Flight: Flight(); break;
                case DragonDrivingStateType.Brakes: Brakes(); break;
                case DragonDrivingStateType.Overdrive: Overdrive(); break;
            }

            // 計算結果を出力
            _rigidbody.drag = _drag;
            _rigidbody.rotation = _rotation;
            _rigidbody.velocity = _velocity;

            // 搭乗者が居ない時はスリープ
            this.enabled = IsMount;
        }

        public void Summon()
        {
            if (Networking.IsOwner(this.gameObject))
            {
                _objectSync.Respawn();
                this.enabled = true;
            }
        }

        /******************************
         入力操作受付
         ******************************/
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

        public void _InputRotateDirect(Vector3 rot)
        {
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
                _rigidbody.AddForce(_jumpSpeed * Vector3.up, ForceMode.VelocityChange);
            }
            else
            {
                _isWalking = false;
            }
        }

        /******************************
         状態別の処理
         ******************************/
        private void Walking()
        {
            _drag = SetDrag(_sqrSpeed);
            _objectSync.SetGravity(true);

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
            //var pitch = Time.deltaTime * noseRotateSpeed * _elevator;
            var yaw = Time.deltaTime * _noseRotateSpeed * _ladder;

            // 入力を反映
            var relativeRotation = Quaternion.Inverse(_rotation) * _noseRotation;
            //relativeRotation = Quaternion.AngleAxis(pitch, Vector3.right) * relativeRotation;
            relativeRotation = Quaternion.AngleAxis(yaw, Vector3.up) * relativeRotation;

            // 進行方向の軸制限
            var relativeDirection = relativeRotation * Vector3.forward;
            if (_groundInfo.collider)
            {
                if (Vector3.Angle(Vector3.up, _groundInfo.normal) < _slopeLimit)
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
            noseDirection = _noseRotation * Vector3.forward;

            // 速度の再計算
            _velocity = turn * _velocity;

            var thrust = Time.deltaTime * _acceleration * _throttle;
            var targetSqrSpeed = _targetVelocity.sqrMagnitude;
            if (_throttle == Vector3.zero)
            {
                if (targetSqrSpeed < _stillSpeedThresholdConverted * _stillSpeedThresholdConverted)
                {
                    _targetVelocity = Vector3.zero;
                    return;
                }
            }
            var velocityForward = Vector3.Project(Quaternion.Inverse(_noseRotation) * _velocity, _targetVelocity);
            var differenceVelocity = _targetVelocity - velocityForward;
            if (differenceVelocity.sqrMagnitude > _accelerateLimit * _accelerateLimit)
            {
                _targetVelocity = Vector3.ClampMagnitude(_targetVelocity, _accelerateLimit) + velocityForward;
            }

            _targetVelocity = Vector3.ClampMagnitude(_targetVelocity + thrust, _maxWalkSpeedConverted);
            _velocity += Time.deltaTime * _acceleration * (_noseRotation * (_targetVelocity - velocityForward));
        }

        private void Hovering()
        {
            _drag = SetDrag(_sqrSpeed);
            _objectSync.SetGravity(false);

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
            var pitch = Time.deltaTime * _noseRotateSpeed * _elevator;
            var yaw = Time.deltaTime * _noseRotateSpeed * _ladder;

            // 入力を反映
            var relativeRotation = Quaternion.Inverse(_rotation) * _noseRotation;
            relativeRotation = Quaternion.AngleAxis(pitch, Vector3.right) * relativeRotation;
            relativeRotation = Quaternion.AngleAxis(yaw, Vector3.up) * relativeRotation;

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
            CalculateVelocity(_maxSpeedConverted);
        }

        private void Flight()
        {
            _drag = SetDrag(_sqrSpeed);
            _objectSync.SetGravity(false);

            // 自動バランサー制御
            var noseDirection = _noseRotation * Vector3.forward;
            var targetRotation = Quaternion.FromToRotation(_rotation * Vector3.forward, noseDirection) * _rotation;
            targetRotation = Quaternion.RotateTowards(_rotation, targetRotation, Time.deltaTime * _stateShiftSpeed);

            var horizontalRotation = Vector3.ProjectOnPlane(_rotation * Vector3.forward, Vector3.up);
            var horizontalTargetRotation = Vector3.ProjectOnPlane(targetRotation * Vector3.forward, Vector3.up);
            var baranceRoll = Vector3.SignedAngle(horizontalTargetRotation, horizontalRotation, Vector3.up);
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

            calculateRotation = Quaternion.AngleAxis(pitch, Vector3.right);
            calculateRotation = _rotation * calculateRotation * Quaternion.Inverse(_rotation);
            _rotation = calculateRotation * _rotation;
            _noseRotation = calculateRotation * _noseRotation;

            var antiRollRotation = Quaternion.LookRotation(_rotation * Vector3.forward);
            var turn = Vector3.Dot(antiRollRotation * Vector3.up, _rotation * Vector3.left);
            turn *= Time.deltaTime * _rollSpeed * _rollToTurnRatio;
            // 本体を旋回
            calculateRotation = Quaternion.AngleAxis(sign * turn, Vector3.up);
            _rotation = calculateRotation * _rotation;
            _noseRotation = calculateRotation * _noseRotation;

            // 進行方向確定
            noseDirection = _noseRotation * Vector3.forward;
            _noseRotation = Quaternion.LookRotation(noseDirection);

            // 速度の再計算
            CalculateVelocity(_maxSpeedConverted);
        }

        private void Brakes()
        {
            _drag = _brakePower;
            _objectSync.SetGravity(false);

            // 自動バランサー制御            
            var forward = _rotation * Vector3.forward;
            var noseDirection = _noseRotation * Vector3.forward;
            var horizontalForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            if (_sqrSpeed < _stillSpeedThresholdConverted * _stillSpeedThresholdConverted)
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
            _targetVelocity = Vector3.ClampMagnitude(velocityForward, _maxSpeedConverted);
            // 本体速度は物理減速に任せる
            //_velocity += Time.deltaTime * _acceleration * (_noseRotation * _targetVelocity - _velocity);
        }

        private void Overdrive()
        {
            _drag = _defaultDrag;
            _objectSync.SetGravity(false);


        }

        /******************************
         その他内部処理
         ******************************/
        private bool CheckGrounded()
        {
            var origin = _rigidbody.position + _rigidbody.rotation * _collider.center;
            return Physics.SphereCast
                    (
                        origin,
                        _groundCheckRadius,
                        Vector3.down,
                        out _groundInfo,
                        _groundCheckRange,
                        _groundLayer,
                        QueryTriggerInteraction.Ignore
                    );
        }

        private float SetDrag(float sqrSpeed)
        {
            if (sqrSpeed < _stillSpeedThresholdConverted * _stillSpeedThresholdConverted)
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

        private void CalculateVelocity(float maxSpeed)
        {
            // 入力値計算
            var thrust = Time.deltaTime * _acceleration * _throttle;

            var targetSqrSpeed = _targetVelocity.sqrMagnitude;
            if (_throttle == Vector3.zero)
            {
                if (targetSqrSpeed < _stillSpeedThresholdConverted * _stillSpeedThresholdConverted)
                {
                    _targetVelocity = Vector3.zero;
                    return;
                }
            }

            var velocityForward = Vector3.Project(Quaternion.Inverse(_noseRotation) * _velocity, _targetVelocity);
            var differenceVelocity = _targetVelocity - velocityForward;
            if (differenceVelocity.sqrMagnitude > _accelerateLimit * _accelerateLimit)
            {
                _targetVelocity = Vector3.ClampMagnitude(_targetVelocity, _accelerateLimit) + velocityForward;
            }

            _targetVelocity = Vector3.ClampMagnitude(_targetVelocity + thrust, maxSpeed);
            _velocity += Time.deltaTime * _acceleration * (_noseRotation * _targetVelocity - _velocity);
        }
    }
}
