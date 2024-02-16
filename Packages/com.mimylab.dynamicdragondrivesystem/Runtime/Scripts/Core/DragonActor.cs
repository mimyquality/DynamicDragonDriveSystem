/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using System.Globalization;

    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [AddComponentMenu("Dynamic Dragon Drive System/Dragon Actor")]
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonActor : UdonSharpBehaviour
    {
        private const float SmoothingDuration = 0.1f;

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
        private byte sync_state;
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

        // Triggerパラメーター
        private int _param_Blink = Animator.StringToHash("OnBlink");
        private int _param_BlinkMembrane = Animator.StringToHash("OnBlinkMembrane");
        private int _param_Collision = Animator.StringToHash("OnCollision");
        // Boolパラメーター
        private int _param_IsOwner = Animator.StringToHash("IsOwner");
        private int _param_IsLocal = Animator.StringToHash("IsLocal");
        private int _param_IsMount = Animator.StringToHash("IsMount");
        private int _param_IsGrounded = Animator.StringToHash("IsGrounded");
        private int _param_IsBrakes = Animator.StringToHash("IsBrakes");
        private int _param_IsOverdrive = Animator.StringToHash("IsOverdrive");
        private int _param_RandomBool = Animator.StringToHash("RandomBool");
        // Intパラメーター
        private int _param_State = Animator.StringToHash("State");
        private int _param_RandomInt = Animator.StringToHash("RandomInt");
        // Floatパラメーター
        private int _param_Pitch = Animator.StringToHash("Pitch");
        private int _param_Roll = Animator.StringToHash("Roll");
        private int _param_NosePitch = Animator.StringToHash("NosePitch");
        private int _param_NoseYaw = Animator.StringToHash("NoseYaw");
        private int _param_VelocityX = Animator.StringToHash("VelocityX");
        private int _param_VelocityY = Animator.StringToHash("VelocityY");
        private int _param_VelocityZ = Animator.StringToHash("VelocityZ");
        private int _param_VelocityMagnitude = Animator.StringToHash("VelocityMagnitude");
        private int _param_Speed = Animator.StringToHash("Speed");
        private int _param_AngularVelocityX = Animator.StringToHash("AngularVelocityX");
        private int _param_AngularVelocityY = Animator.StringToHash("AngularVelocityY");
        private int _param_AngularVelocityZ = Animator.StringToHash("AngularVelocityZ");
        private int _param_AngularVelocityMagnitude = Animator.StringToHash("AngularVelocityMagnitude");
        private int _param_AngularSpeed = Animator.StringToHash("AngularSpeed");
        private int _param_RandomFloat = Animator.StringToHash("RandomFloat");

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _animator = GetComponent<Animator>();
            _rigidbody = driver.GetComponent<Rigidbody>();

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
            _animator.SetTrigger(_param_Blink);

            var nextTiming = Random.Range(_blinkRateMin, _blinkRateMax);
            SendCustomEventDelayedSeconds(nameof(_TriggerBlink), nextTiming);
        }

        public void _TriggerBlinkMembrane()
        {
            _animator.SetTrigger(_param_BlinkMembrane);

            var nextTiming = Random.Range(_blinkMembraneRateMin, _blinkMembraneRateMax);
            SendCustomEventDelayedSeconds(nameof(_TriggerBlinkMembrane), nextTiming);
        }

        public void _TriggerCollision()
        {
            if (!_initialized) { return; }

            _animator.SetTrigger(_param_Collision);
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

            if (sync_state != (byte)driver.State)
            {
                sync_state = (byte)driver.State;
                RequestSerialization();
            }

            var noseForward = Quaternion.Inverse(_rotation) * driver.NoseRotation * Vector3.forward;
            var axisDirection = Vector3.ProjectOnPlane(noseForward, Vector3.left);
            _noseDirection.x = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.left);
            axisDirection = Vector3.ProjectOnPlane(noseForward, Vector3.up);
            _noseDirection.y = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.up);
            if (sync_noseDirection != _noseDirection)
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
            _animator.SetBool(_param_IsOwner, _isOwner);
            _animator.SetBool(_param_IsLocal, _isOwner);
            _animator.SetBool(_param_IsMount, isMount);
            _animator.SetBool(_param_IsGrounded, _isGrounded);
            _animator.SetBool(_param_IsBrakes, sync_isBrakes);
            _animator.SetBool(_param_IsOverdrive, sync_isOverdrive);
            _animator.SetBool(_param_RandomBool, _randomBool);

            _animator.SetInteger(_param_State, (int)sync_state);
            _animator.SetInteger(_param_RandomInt, _randomInt);

            _animator.SetFloat(_param_NosePitch, _noseDirection.x);
            _animator.SetFloat(_param_NoseYaw, _noseDirection.y);
            _animator.SetFloat(_param_Pitch, _pitch);
            _animator.SetFloat(_param_Roll, _roll);
            _animator.SetFloat(_param_VelocityX, _relativeVelocity.x);
            _animator.SetFloat(_param_VelocityY, _relativeVelocity.y);
            _animator.SetFloat(_param_VelocityZ, _relativeVelocity.z);
            _animator.SetFloat(_param_VelocityMagnitude, _speed);
            _animator.SetFloat(_param_Speed, _speed);
            _animator.SetFloat(_param_AngularVelocityX, _relativeAngularVelocity.x);
            _animator.SetFloat(_param_AngularVelocityY, _relativeAngularVelocity.y);
            _animator.SetFloat(_param_AngularVelocityZ, _relativeAngularVelocity.z);
            _animator.SetFloat(_param_AngularVelocityMagnitude, _angularSpeed);
            _animator.SetFloat(_param_AngularSpeed, _angularSpeed);
            _animator.SetFloat(_param_RandomFloat, _randomFloat);
        }
    }
}
