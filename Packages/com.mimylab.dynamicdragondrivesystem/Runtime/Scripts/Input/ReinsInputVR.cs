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

        private Vector3 _rightGrabMove, _leftGrgabMove;
        private Vector3 _rightGrabRotate, _leftGrabRotate;
        private bool _prevGrabJump;

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
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
            _originPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).position;
            _originRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).rotation;
            _rightGrabMove = GetRightGrabMove();
            _leftGrgabMove = GetLeftGrabMove();
            _rightGrabRotate = GetRightGrabRotate();
            _leftGrabRotate = GetLeftGrabRotate();

            InputGrabJump();

            _brakes = (_leftGrgabMove.z < -_brakesAcceptanceThreshold) && (_rightGrabMove.z < -_brakesAcceptanceThreshold);
            //_turbo = 

            _thrust = (_throttleInputHand == HandType.LEFT) ? _leftGrgabMove.z : _rightGrabMove.z;
            //_lift = (_throttleInputHand == HandType.LEFT) ? _leftGrgabMove.y : _rightGrabMove.y;
            //_lateral = (_throttleInputHand == HandType.LEFT) ? _leftGrgabMove.x : _rightGrabMove.x;

            _elevator = (_elevatorInputHand == HandType.LEFT) ? _leftGrabRotate.x : _rightGrabRotate.x;
            _ladder = (_turningInputHand == HandType.LEFT) ? _leftGrabRotate.y : _rightGrabRotate.y;
            _aileron = (_turningInputHand == HandType.LEFT) ? _leftGrabRotate.z : _rightGrabRotate.z;

            _flagGrabLeft = false;
            _flagGrabRight = false;
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

        private Vector3 GetRightGrabMove()
        {
            var result = Vector3.zero;
            if (!_isGrabRight) { return result; }

            var avatarScale = _localPlayer.GetAvatarEyeHeightAsMeters();
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

        private Vector3 GetLeftGrabMove()
        {
            var result = Vector3.zero;
            if (!_isGrabLeft) { return result; }

            var avatarScale = _localPlayer.GetAvatarEyeHeightAsMeters();
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

        private Vector3 GetRightGrabRotate()
        {
            var result = Vector3.zero;
            if (!_isGrabRight) { return result; }

            var handRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation;
            handRotation = Quaternion.Inverse(_originRotation) * handRotation;

            if (_flagGrabRight)
            {
                _rightGrabRotation = handRotation;
            }

            var relativeGrabRotation = handRotation * Quaternion.Inverse(_rightGrabRotation);
            var relativeGrabDirection = relativeGrabRotation * Vector3.forward;

            var axisDirection = Vector3.ProjectOnPlane(relativeGrabDirection, Vector3.right);
            var angle = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.right);
            result.x = angle / _rotateRatio;

            axisDirection = Vector3.ProjectOnPlane(relativeGrabDirection, Vector3.up);
            angle = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.up);
            result.y = angle / _rotateRatio;

            angle = Vector3.SignedAngle(Quaternion.LookRotation(relativeGrabDirection) * Vector3.up, relativeGrabRotation * Vector3.up, relativeGrabDirection);
            result.z = angle / _rotateRatio;

            return result;
        }

        private Vector3 GetLeftGrabRotate()
        {
            var result = Vector3.zero;
            if (!_isGrabLeft) { return result; }

            var handRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).rotation;
            handRotation = Quaternion.Inverse(_originRotation) * handRotation;

            if (_flagGrabLeft)
            {
                _leftGrabRotation = handRotation;
            }

            var relativeGrabRotation = handRotation * Quaternion.Inverse(_leftGrabRotation);
            var relativeGrabDirection = relativeGrabRotation * Vector3.forward;

            var axisDirection = Vector3.ProjectOnPlane(relativeGrabDirection, Vector3.right);
            var angle = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.right);
            result.x = angle / _rotateRatio;

            axisDirection = Vector3.ProjectOnPlane(relativeGrabDirection, Vector3.up);
            angle = Vector3.SignedAngle(Vector3.forward, axisDirection, Vector3.up);
            result.y = angle / _rotateRatio;

            angle = Vector3.SignedAngle(Quaternion.LookRotation(relativeGrabDirection) * Vector3.up, relativeGrabRotation * Vector3.up, relativeGrabDirection);
            result.z = angle / _rotateRatio;

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
