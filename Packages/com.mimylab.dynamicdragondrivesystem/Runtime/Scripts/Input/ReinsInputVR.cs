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
    using VRC.Udon.Common;
    //using VRC.SDK3.Components;

    [AddComponentMenu("Dynamic Dragon Drive System/ReinsInput VRHands")]
    [DefaultExecutionOrder(-100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsInputVR : ReinsInputManager
    {
        [Tooltip("meter"), SerializeField]
        private float _moveScale = 0.2f;
        [Tooltip("radius"), SerializeField]
        private float _rotateRatio = 60.0f;

        [Space]
        [Range(0.0f, 1.0f), SerializeField]
        private float _jumpAcceptanceThreshold = 0.9f;
        [Range(0.0f, 1.0f), SerializeField]
        private float _brakesAcceptanceThreshold = 0.9f;

        private VRCPlayerApi _localPlayer;
        private bool _isGrabLeft, _isGrabRight;
        private bool _flagGrabLeft, _flagGrabRight;
        private Vector3 _originPosition, _leftGrabPosition, _rightGrabPosition;
        private Quaternion _originRotation, _leftGrabRotation, _rightGrabRotation;
        private Quaternion _leftHandRotation, _prevLeftHandRotation;
        private Quaternion _rightHandRotation, _prevRightHandRotation;

        private Vector3 _leftGrgabMove, _rightGrabMove;
        private Vector3 _leftGrabRotate, _rightGrabRotate;
        private Vector3 _leftHandRotate, _rightHandRotate;
        private bool _prevGrabJump;

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
        }

        public override void PostLateUpdate()
        {
            if (!this.enabled) { return; }

            _originPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).position;
            _originRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).rotation;
            _leftGrgabMove = GetLeftGrabMove();
            _rightGrabMove = GetRightGrabMove();
            _leftGrabRotate = ConvertQuaternionToAngle3(GetLeftGrabRotate());
            _rightGrabRotate = ConvertQuaternionToAngle3(GetRightGrabRotate());

            _leftHandRotate = ConvertQuaternionToAngle3(GetLeftHandRotate());
            _rightHandRotate = ConvertQuaternionToAngle3(GetRightHandRotate());

            _flagGrabLeft = false;
            _flagGrabRight = false;
            _prevLeftHandRotation = _leftHandRotation;
            _prevRightHandRotation = _rightHandRotation;
        }

        public override void InputGrab(bool value, UdonInputEventArgs args)
        {
            switch (args.handType)
            {
                case HandType.RIGHT: ActivateGrabRight(value); break;
                case HandType.LEFT: ActivateGrabLeft(value); break;
            }
        }

        protected override void InputKey()
        {
            InputGrabJump();

            _brakes = (_leftGrgabMove.z < -_brakesAcceptanceThreshold) && (_rightGrabMove.z < -_brakesAcceptanceThreshold);
            //_turbo = 

            _thrust = (_throttleInputHand == HandType.LEFT) ? _leftGrgabMove.z : _rightGrabMove.z;
            //_lift = (_throttleInputHand == HandType.LEFT) ? _leftGrgabMove.y : _rightGrabMove.y;
            //_lateral = (_throttleInputHand == HandType.LEFT) ? _leftGrgabMove.x : _rightGrabMove.x;

            _elevator = (_elevatorInputHand == HandType.LEFT) ? _leftGrabRotate.x : _rightGrabRotate.x;
            _elevator = Mathf.Clamp(_elevator / _rotateRatio, -1.0f, 1.0f);
            _ladder = (_turningInputHand == HandType.LEFT) ? _leftGrabRotate.y : _rightGrabRotate.y;
            _ladder = Mathf.Clamp(_ladder / _rotateRatio, -1.0f, 1.0f);
            _aileron = (_turningInputHand == HandType.LEFT) ? _leftGrabRotate.z : _rightGrabRotate.z;
            _aileron = Mathf.Clamp(_aileron / _rotateRatio, -1.0f, 1.0f);

            var pitch = (_elevatorInputHand == HandType.LEFT) ? _leftHandRotate.x : _rightHandRotate.x;
            var yaw = (_turningInputHand == HandType.LEFT) ? _leftHandRotate.y : _rightHandRotate.y;
            var roll = (_turningInputHand == HandType.LEFT) ? _leftHandRotate.z : _rightHandRotate.z;
            driver._InputDirectRotate(new Vector3(pitch, yaw, roll));
        }

        private void ActivateGrabLeft(bool value)
        {
            if (value & !_isGrabLeft) { _flagGrabLeft = true; }
            _isGrabLeft = value;
        }

        private void ActivateGrabRight(bool value)
        {
            if (value & !_isGrabRight) { _flagGrabRight = true; }
            _isGrabRight = value;
        }

        private Vector3 GetLeftGrabMove()
        {
            var result = Vector3.zero;
            if (!_isGrabLeft) { return result; }

            var avatarScale = Mathf.Max(_localPlayer.GetAvatarEyeHeightAsMeters(), 0.1f);
            var handPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
            handPosition = Quaternion.Inverse(_originRotation) * (handPosition - _originPosition);

            if (_flagGrabLeft)
            {
                _leftGrabPosition = handPosition;
            }
            result = handPosition - _leftGrabPosition;
            result = result / (avatarScale * _moveScale);

            return result;
        }

        private Vector3 GetRightGrabMove()
        {
            var result = Vector3.zero;
            if (!_isGrabRight) { return result; }

            var avatarScale = Mathf.Max(_localPlayer.GetAvatarEyeHeightAsMeters(), 0.1f);
            var handPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            handPosition = Quaternion.Inverse(_originRotation) * (handPosition - _originPosition);

            if (_flagGrabRight)
            {
                _rightGrabPosition = handPosition;
            }
            result = handPosition - _rightGrabPosition;
            result = result / (avatarScale * _moveScale);

            return result;
        }

        private Quaternion GetLeftGrabRotate()
        {
            if (!_isGrabLeft) { return Quaternion.identity; }

            _leftHandRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).rotation;
            _leftHandRotation = Quaternion.Inverse(_originRotation) * _leftHandRotation;

            if (_flagGrabLeft)
            {
                _leftGrabRotation = _leftHandRotation;
                _prevLeftHandRotation = _leftHandRotation;
            }

            return _leftHandRotation * Quaternion.Inverse(_leftGrabRotation);
        }

        private Quaternion GetRightGrabRotate()
        {
            if (!_isGrabRight) { return Quaternion.identity; }

            _rightHandRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation;
            _rightHandRotation = Quaternion.Inverse(_originRotation) * _rightHandRotation;

            if (_flagGrabRight)
            {
                _rightGrabRotation = _rightHandRotation;
                _prevRightHandRotation = _rightHandRotation;
            }

            return _rightHandRotation * Quaternion.Inverse(_rightGrabRotation);
        }

        private Quaternion GetLeftHandRotate()
        {
            if (!_isGrabLeft) { return Quaternion.identity; }

            return _leftHandRotation * Quaternion.Inverse(_prevLeftHandRotation);
        }

        private Quaternion GetRightHandRotate()
        {
            if (!_isGrabRight) { return Quaternion.identity; }

            return _rightHandRotation * Quaternion.Inverse(_prevRightHandRotation);
        }

        private Vector3 ConvertQuaternionToAngle3(Quaternion rotation)
        {
            var result = Vector3.zero;
            if (rotation == Quaternion.identity) { return result; }

            var rotateDirection = rotation * Vector3.forward;

            var axisDirection = Vector3.ProjectOnPlane(rotateDirection, Vector3.right);
            var angle = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.right);
            result.x = angle;

            axisDirection = Vector3.ProjectOnPlane(rotateDirection, Vector3.up);
            angle = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.up);
            result.y = angle;

            angle = Vector3.SignedAngle(Quaternion.LookRotation(rotateDirection) * Vector3.up, rotation * Vector3.up, rotateDirection);
            result.z = angle;

            return result;
        }

        private void InputGrabJump()
        {
            var grabJump = _leftGrgabMove.y > _jumpAcceptanceThreshold;
            if (grabJump && !_prevGrabJump)
            {
                driver._InputJump();
            }
            _prevGrabJump = grabJump;
        }
    }
}
