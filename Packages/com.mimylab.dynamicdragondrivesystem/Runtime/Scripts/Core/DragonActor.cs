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
    //using VRC.SDK3.Components;

    public enum DragonActorParameterName
    {
        // Triggerパラメーター
        OnBlink, OnBlinkMembrane,
        OnCollision,
        // Boolパラメーター
        IsOwner, IsLocal,
        IsMount,
        IsGrounded,
        IsBrakes,
        IsOverdrive,
        RandomBool,
        // Intパラメーター
        State,
        RandomInt,
        // Floatパラメーター
        Pitch,
        Roll,
        NosePitch,
        NoseYaw,
        VelocityX, VelocityY, VelocityZ,
        VelocityMagnitude, Speed,
        AngularVelocityX, AngularVelocityY, AngularVelocityZ,
        AngularVelocityMagnitude, AngularSpeed,
        RandomFloat
    }

    [AddComponentMenu("Dynamic Dragon Drive System/Dragon Actor")]
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonActor : UdonSharpBehaviour
    {
        private const float SmoothingDuration = 0.1f;   // 単位：sec
        private const float AngleSyncTolerance = 0.1f; // 単位：°

        internal DragonDriver driver;
        internal bool isMount;

        [SerializeField, Min(0.0f), Tooltip("sec")]
        private float _blinkRateMin = 20.0f;
        [SerializeField, Min(0.0f), Tooltip("sec")]
        private float _blinkRateMax = 60.0f;
        [SerializeField, Min(0.0f), Tooltip("sec")]
        private float _blinkMembraneRateMin = 4.0f;
        [SerializeField, Min(0.0f), Tooltip("sec")]
        private float _blinkMembraneRateMax = 20.0f;

        [UdonSynced]
        private bool sync_isBrakes;
        [UdonSynced]
        private bool sync_isOverdrive;
        [UdonSynced]
        private int sync_state;
        [UdonSynced]
        private Vector2 sync_noseDirection;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private Quaternion _rotation;

        private bool _isOwner, _isGrounded;
        private Vector2 _noseDirection;
        private Vector2 _noseVelocity;
        private float _pitch, _roll;
        private Vector3 _relativeVelocity;
        private Vector3 _relativeAngularVelocity;
        private float _speed, _angularSpeed;
        private bool _randomBool;
        private int _randomInt;
        private float _randomFloat;

        private int[] _parametersHash =
        {
            // Triggerパラメーター
            Animator.StringToHash(DragonActorParameterName.OnBlink.ToString()),
            Animator.StringToHash(DragonActorParameterName.OnBlinkMembrane.ToString()),
            Animator.StringToHash(DragonActorParameterName.OnCollision.ToString()),
            // Boolパラメーター
            Animator.StringToHash(DragonActorParameterName.IsOwner.ToString()),
            Animator.StringToHash(DragonActorParameterName.IsLocal.ToString()),
            Animator.StringToHash(DragonActorParameterName.IsMount.ToString()),
            Animator.StringToHash(DragonActorParameterName.IsGrounded.ToString()),
            Animator.StringToHash(DragonActorParameterName.IsBrakes.ToString()),
            Animator.StringToHash(DragonActorParameterName.IsOverdrive.ToString()),
            Animator.StringToHash(DragonActorParameterName.RandomBool.ToString()),
            // Intパラメーター
            Animator.StringToHash(DragonActorParameterName.State.ToString()),
            Animator.StringToHash(DragonActorParameterName.RandomInt.ToString()),
            // Floatパラメーター
            Animator.StringToHash(DragonActorParameterName.Pitch.ToString()),
            Animator.StringToHash(DragonActorParameterName.Roll.ToString()),
            Animator.StringToHash(DragonActorParameterName.NosePitch.ToString()),
            Animator.StringToHash(DragonActorParameterName.NoseYaw.ToString()),
            Animator.StringToHash(DragonActorParameterName.VelocityX.ToString()),
            Animator.StringToHash(DragonActorParameterName.VelocityY.ToString()),
            Animator.StringToHash(DragonActorParameterName.VelocityZ.ToString()),
            Animator.StringToHash(DragonActorParameterName.VelocityMagnitude.ToString()),
            Animator.StringToHash(DragonActorParameterName.Speed.ToString()),
            Animator.StringToHash(DragonActorParameterName.AngularVelocityX.ToString()),
            Animator.StringToHash(DragonActorParameterName.AngularVelocityY.ToString()),
            Animator.StringToHash(DragonActorParameterName.AngularVelocityZ.ToString()),
            Animator.StringToHash(DragonActorParameterName.AngularVelocityMagnitude.ToString()),
            Animator.StringToHash(DragonActorParameterName.AngularSpeed.ToString()),
            Animator.StringToHash(DragonActorParameterName.RandomFloat.ToString())
        };
        private bool[] _parametersValid;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _animator = GetComponent<Animator>();
            _rigidbody = driver.GetComponent<Rigidbody>();

            _parametersValid = ValidateParameters(_parametersHash, _animator);

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            _TriggerBlink();
            _TriggerBlinkMembrane();
        }

        private void Update()
        {
            _rotation = _rigidbody.rotation;
            _isOwner = Networking.IsOwner(this.gameObject);
            UpdateSyncParameters();
            CalculateParameters();
            SetAnimatorParameters();
        }

        // Animator.applyRootMotionを強制的にオフにする
        private void OnAnimatorMove() { }

        public void _GenerateRandomBool()
        {
            _randomBool = Random.value > 0.5f;
        }

        public void _GenerateRandomInt()
        {
            _randomInt = Random.Range(-1, byte.MaxValue) + 1;
        }

        public void _GenerateRandomFloat()
        {
            _randomFloat = Random.value;
        }

        public void _TriggerBlink()
        {
            if (!_parametersValid[(int)DragonActorParameterName.OnBlink]) { return; }

            _animator.SetTrigger(_parametersHash[(int)DragonActorParameterName.OnBlink]);

            var nextTiming = Random.Range(_blinkRateMin, _blinkRateMax);
            SendCustomEventDelayedSeconds(nameof(_TriggerBlink), nextTiming);
        }

        public void _TriggerBlinkMembrane()
        {
            if (!_parametersValid[(int)DragonActorParameterName.OnBlinkMembrane]) { return; }

            _animator.SetTrigger(_parametersHash[(int)DragonActorParameterName.OnBlinkMembrane]);

            var nextTiming = Random.Range(_blinkMembraneRateMin, _blinkMembraneRateMax);
            SendCustomEventDelayedSeconds(nameof(_TriggerBlinkMembrane), nextTiming);
        }

        public void _TriggerCollision()
        {
            if (!_initialized) { return; }
            if (!_parametersValid[(int)DragonActorParameterName.OnCollision]) { return; }

            _animator.SetTrigger(_parametersHash[(int)DragonActorParameterName.OnCollision]);
        }

        static private bool[] ValidateParameters(int[] parametersHash, Animator animator)
        {
            var result = new bool[parametersHash.Length];

            var parameters = animator.parameters;
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < parameters.Length; j++)
                {
                    if (parametersHash[i] == parameters[j].nameHash)
                    {
                        result[i] = true;
                        break;
                    }
                }
            }

            return result;
        }

        private void UpdateSyncParameters()
        {
            if (!_isOwner) { return; }

            if (sync_isBrakes != driver.IsBrakes)
            {
                sync_isBrakes = !sync_isBrakes;
                RequestSerialization();
            }
            if (sync_isOverdrive != driver.IsOverdrive)
            {
                sync_isOverdrive = !sync_isOverdrive;
                RequestSerialization();
            }

            if (sync_state != driver.State)
            {
                sync_state = driver.State;
                RequestSerialization();
            }

            var noseForward = driver.NoseDirection;
            var axisDirection = Vector3.ProjectOnPlane(noseForward, Vector3.left);
            _noseDirection.x = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.left);
            axisDirection = Vector3.ProjectOnPlane(noseForward, Vector3.up);
            _noseDirection.y = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.up);
            if (!((Mathf.Abs(sync_noseDirection.x - _noseDirection.x) < AngleSyncTolerance)
               && (Mathf.Abs(sync_noseDirection.y - _noseDirection.y) < AngleSyncTolerance)))
            {
                sync_noseDirection = _noseDirection;
                RequestSerialization();
            }
        }

        private void CalculateParameters()
        {
            _isGrounded = driver.IsGrounded;

            if (!_isOwner)
            {
                _noseDirection = Vector2.SmoothDamp(_noseDirection, sync_noseDirection, ref _noseVelocity, SmoothingDuration);
            }

            var forward = _rotation * Vector3.forward;
            var left = _rotation * Vector3.left;
            var up = _rotation * Vector3.up;

            var level = Vector3.ProjectOnPlane(forward, Vector3.up);
            _pitch = Vector3.SignedAngle(level, forward, left);

            level = Quaternion.LookRotation(forward) * Vector3.up;
            _roll = Vector3.SignedAngle(level, up, forward);

            _relativeVelocity = Quaternion.Inverse(_rotation) * driver.velocity;
            _relativeAngularVelocity = Quaternion.Inverse(_rotation) * driver.angularVelocity;

            _speed = _relativeVelocity.magnitude;
            _angularSpeed = _relativeAngularVelocity.magnitude;
        }

        private void SetAnimatorParameters()
        {
            if (_parametersValid[(int)DragonActorParameterName.IsOwner]) { _animator.SetBool(_parametersHash[(int)DragonActorParameterName.IsOwner], _isOwner); }
            if (_parametersValid[(int)DragonActorParameterName.IsLocal]) _animator.SetBool(_parametersHash[(int)DragonActorParameterName.IsLocal], _isOwner);
            if (_parametersValid[(int)DragonActorParameterName.IsMount]) _animator.SetBool(_parametersHash[(int)DragonActorParameterName.IsMount], isMount);
            if (_parametersValid[(int)DragonActorParameterName.IsGrounded]) _animator.SetBool(_parametersHash[(int)DragonActorParameterName.IsGrounded], _isGrounded);
            if (_parametersValid[(int)DragonActorParameterName.IsBrakes]) _animator.SetBool(_parametersHash[(int)DragonActorParameterName.IsBrakes], sync_isBrakes);
            if (_parametersValid[(int)DragonActorParameterName.IsOverdrive]) _animator.SetBool(_parametersHash[(int)DragonActorParameterName.IsOverdrive], sync_isOverdrive);
            if (_parametersValid[(int)DragonActorParameterName.RandomBool]) _animator.SetBool(_parametersHash[(int)DragonActorParameterName.RandomBool], _randomBool);

            if (_parametersValid[(int)DragonActorParameterName.State]) _animator.SetInteger(_parametersHash[(int)DragonActorParameterName.State], sync_state);
            if (_parametersValid[(int)DragonActorParameterName.RandomInt]) _animator.SetInteger(_parametersHash[(int)DragonActorParameterName.RandomInt], _randomInt);

            if (_parametersValid[(int)DragonActorParameterName.NosePitch]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.NosePitch], _noseDirection.x);
            if (_parametersValid[(int)DragonActorParameterName.NoseYaw]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.NoseYaw], _noseDirection.y);
            if (_parametersValid[(int)DragonActorParameterName.Pitch]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.Pitch], _pitch);
            if (_parametersValid[(int)DragonActorParameterName.Roll]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.Roll], _roll);
            if (_parametersValid[(int)DragonActorParameterName.VelocityX]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.VelocityX], _relativeVelocity.x);
            if (_parametersValid[(int)DragonActorParameterName.VelocityY]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.VelocityY], _relativeVelocity.y);
            if (_parametersValid[(int)DragonActorParameterName.VelocityZ]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.VelocityZ], _relativeVelocity.z);
            if (_parametersValid[(int)DragonActorParameterName.VelocityMagnitude]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.VelocityMagnitude], _speed);
            if (_parametersValid[(int)DragonActorParameterName.Speed]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.Speed], _speed);
            if (_parametersValid[(int)DragonActorParameterName.AngularVelocityX]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.AngularVelocityX], _relativeAngularVelocity.x);
            if (_parametersValid[(int)DragonActorParameterName.AngularVelocityY]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.AngularVelocityY], _relativeAngularVelocity.y);
            if (_parametersValid[(int)DragonActorParameterName.AngularVelocityZ]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.AngularVelocityZ], _relativeAngularVelocity.z);
            if (_parametersValid[(int)DragonActorParameterName.AngularVelocityMagnitude]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.AngularVelocityMagnitude], _angularSpeed);
            if (_parametersValid[(int)DragonActorParameterName.AngularSpeed]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.AngularSpeed], _angularSpeed);
            if (_parametersValid[(int)DragonActorParameterName.RandomFloat]) _animator.SetFloat(_parametersHash[(int)DragonActorParameterName.RandomFloat], _randomFloat);
        }
    }
}
