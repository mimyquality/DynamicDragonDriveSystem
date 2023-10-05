/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class DragonActor : UdonSharpBehaviour
    {
        public DragonDriver driver;
        public bool isMount;

        public Transform nose;  // Debug
        public GameObject groundedLamp; //Debug
        public Transform speedLamp; //Debug
        public Transform targetSpeedLamp; //Debug

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
        private Vector3 _revPosition;
        private Quaternion _revRotation;
        private Vector3 _relativeVelocity, _relativeAngularVelocity;

        // Boolパラメーター
        private int _param_IsMount = Animator.StringToHash("IsMount");
        private int _param_IsGrounded = Animator.StringToHash("IsGrounded");
        private int _param_IsBrakes = Animator.StringToHash("IsBrakes");
        private int _param_IsOverdrive = Animator.StringToHash("IsOverdrive");
        // Intパラメーター
        private int _param_State = Animator.StringToHash("State");
        // Floatパラメーター
        private int _param_Pitch = Animator.StringToHash("Pitch");
        private int _param_Roll = Animator.StringToHash("Roll");
        private int _param_NosePitch = Animator.StringToHash("NosePitch");
        private int _param_NoseYaw = Animator.StringToHash("NoseYaw");
        private int _param_VelocityX = Animator.StringToHash("VelocityX");
        private int _param_VelocityY = Animator.StringToHash("VelocityY");
        private int _param_VelocityZ = Animator.StringToHash("VelocityZ");
        private int _param_VelocityMagnitude = Animator.StringToHash("VelocityMagnitude");

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _localPlayer = Networking.LocalPlayer;
            _animator = GetComponent<Animator>();
            _rigidbody = driver.GetComponent<Rigidbody>();
            _revPosition = _rigidbody.position;
            _revRotation = _rigidbody.rotation;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            UpdateSyncedParameters();
            CalculateParameters();
            SetAnimatorParameters();

            // Debug
            nose.rotation = driver.NoseRotation;
            groundedLamp.SetActive(driver.IsGrounded);

            var speedToLocalPosition = speedLamp.localPosition;
            speedToLocalPosition.y = _rigidbody.velocity.magnitude / driver.MaxSpeed;
            speedLamp.localPosition = speedToLocalPosition;

            var targetSpeedToLocalPosition = targetSpeedLamp.localPosition;
            targetSpeedToLocalPosition.y = driver.TargetSpeed / driver.MaxSpeed;
            targetSpeedLamp.localPosition = targetSpeedToLocalPosition;
            // End Debug
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

            var velocity = (_rigidbody.position - _revPosition) / Time.deltaTime;
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
            _animator.SetInteger(_param_State, (int)_state);
            _animator.SetFloat(_param_NosePitch, _noseDirection.x);
            _animator.SetFloat(_param_NoseYaw, _noseDirection.y);
            _animator.SetFloat(_param_Pitch, _pitch);
            _animator.SetFloat(_param_Roll, _roll);
            _animator.SetFloat(_param_VelocityX, _relativeVelocity.x);
            _animator.SetFloat(_param_VelocityY, _relativeVelocity.y);
            _animator.SetFloat(_param_VelocityZ, _relativeVelocity.z);
            _animator.SetFloat(_param_VelocityMagnitude, _relativeVelocity.magnitude);
        }
    }
}
