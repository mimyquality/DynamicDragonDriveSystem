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

    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class DragonActor : UdonSharpBehaviour
    {
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
        [SerializeField, Min(0.0f), Tooltip("m/s")]
        private float _collisionTriggerVelocity = 8.0f;
        [SerializeField, Range(0.0f, 180.0f), Tooltip("degree")]
        private float _collisionIncidenceAngle = 60.0f;

        private VRCPlayerApi _localPlayer;
        private Animator _animator;
        private Rigidbody _rigidbody;

        [UdonSynced]
        private bool _isGrounded, _isBrakes, _isOverdrive;
        [UdonSynced]
        private byte _state;
        [UdonSynced(UdonSyncMode.Linear)]
        private Vector2 _noseDirection;

        private float _pitch, _roll;
        private int _velocitySmoothingRange = 30;
        private Vector3 _revPosition;
        private Quaternion _revRotation;
        private Vector3[] _revVelocity;
        private Vector3 _relativeVelocity, _relativeAngularVelocity;
        private bool _randomBool;
        private int _randomInt;
        private float _randomFloat;

        // Triggerパラメーター
        private int _param_Blink = Animator.StringToHash("OnBlink");
        private int _param_BlinkMembrane = Animator.StringToHash("OnBlinkMembrane");
        private int _param_Collision = Animator.StringToHash("OnCollision");
        // Boolパラメーター
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
        private int _param_RandomFloat = Animator.StringToHash("RandomFloat");

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _localPlayer = Networking.LocalPlayer;
            _animator = GetComponent<Animator>();
            _rigidbody = driver.GetComponent<Rigidbody>();

            _revPosition = _rigidbody.position;
            _revRotation = _rigidbody.rotation;
            _revVelocity = new Vector3[_velocitySmoothingRange];

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
            UpdateSyncedParameters();
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

        public void _TriggerCollision(Collision obstacle)
        {
            if (!_initialized) { return; }
            if (_relativeVelocity.sqrMagnitude < _collisionTriggerVelocity * _collisionTriggerVelocity) { return; }
            for (int i = 0; i < obstacle.contactCount; i++)
            {
                if (Vector3.Angle(_rigidbody.rotation * Vector3.up, obstacle.contacts[0].normal) < _collisionIncidenceAngle)
                {
                    return;
                }
            }

            _animator.SetTrigger(_param_Collision);
        }

        private void UpdateSyncedParameters()
        {
            if (!_localPlayer.IsOwner(this.gameObject)) { return; }

            _isGrounded = driver.IsGrounded;
            _isBrakes = driver.IsBrakes;
            _isOverdrive = driver.IsOverdrive;
            _state = (byte)driver.State;

            var noseForward = Quaternion.Inverse(_rigidbody.rotation) * driver.NoseRotation * Vector3.forward;
            var AxisDirection = Vector3.ProjectOnPlane(noseForward, Vector3.left);
            _noseDirection.x = Vector3.SignedAngle(Vector3.forward, AxisDirection, Vector3.left);
            AxisDirection = Vector3.ProjectOnPlane(noseForward, Vector3.up);
            _noseDirection.y = Vector3.SignedAngle(Vector3.forward, AxisDirection, Vector3.up);
        }

        private void CalculateParameters()
        {
            var forward = _rigidbody.rotation * Vector3.forward;
            var left = _rigidbody.rotation * Vector3.left;
            var up = _rigidbody.rotation * Vector3.up;

            var datum = Vector3.ProjectOnPlane(forward, Vector3.up);
            _pitch = Vector3.SignedAngle(datum, forward, left);

            datum = Quaternion.LookRotation(forward) * Vector3.up;
            _roll = Vector3.SignedAngle(datum, up, forward);

            System.Array.Copy(_revVelocity, 0, _revVelocity, 1, _revVelocity.Length - 1);
            _revVelocity[0] = (_rigidbody.position - _revPosition) / Time.deltaTime;

            Vector3 velocity = Vector3.zero;
            for (int i = 0; i < _revVelocity.Length; i++)
            {
                velocity += _revVelocity[i];
            }
            velocity /= _velocitySmoothingRange;
            _relativeVelocity = Quaternion.Inverse(_rigidbody.rotation) * velocity;

            float rotateAngle;
            Vector3 rotateAxis;
            var deltaRotate = Quaternion.Inverse(_revRotation) * _rigidbody.rotation;
            deltaRotate.ToAngleAxis(out rotateAngle, out rotateAxis);
            _relativeAngularVelocity = rotateAxis * (rotateAngle / Time.deltaTime);

            _revPosition = _rigidbody.position;
            _revRotation = _rigidbody.rotation;
        }

        private void SetAnimatorParameters()
        {
            _animator.SetBool(_param_IsMount, isMount);
            _animator.SetBool(_param_IsGrounded, _isGrounded);
            _animator.SetBool(_param_IsBrakes, _isBrakes);
            _animator.SetBool(_param_IsOverdrive, _isOverdrive);
            _animator.SetBool(_param_RandomBool, _randomBool);
            _animator.SetInteger(_param_State, (int)_state);
            _animator.SetInteger(_param_RandomInt, _randomInt);
            _animator.SetFloat(_param_NosePitch, _noseDirection.x);
            _animator.SetFloat(_param_NoseYaw, _noseDirection.y);
            _animator.SetFloat(_param_Pitch, _pitch);
            _animator.SetFloat(_param_Roll, _roll);
            _animator.SetFloat(_param_VelocityX, _relativeVelocity.x);
            _animator.SetFloat(_param_VelocityY, _relativeVelocity.y);
            _animator.SetFloat(_param_VelocityZ, _relativeVelocity.z);
            _animator.SetFloat(_param_VelocityMagnitude, _relativeVelocity.magnitude);
            _animator.SetFloat(_param_RandomFloat, _randomFloat);
        }
    }
}
