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

    [AddComponentMenu("Dynamic Dragon Drive System/ReinsInput VRHands2")]
    public class ReinsInputVR2 : ReinsInputManager
    {
        [SerializeField, Tooltip("meter")]
        private float _moveScale = 0.2f;
        [SerializeField, Tooltip("radius")]
        private float _rotateRatio = 90.0f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float _rotateDirection = 0.5f;

        [Space]
        [SerializeField, Range(0.0f, 1.0f)]
        private float _jumpAcceptanceThreshold = 0.95f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float _brakesAcceptanceThreshold = 0.95f;

        private VRCPlayerApi _localPlayer;
        private bool _isGrabLeft, _isGrabRight, _isGrabBoth;
        private bool _flagGrabBoth;
        private Vector3 _originPosition, _leftHandPosition, _rightHandPosition;
        private Quaternion _originRotation;

        private Vector3 _centerGrabPosition;
        private Vector3 _centerGrabDirection;
        private Vector3 _centerGrabRotateAxis;

        private Vector3 _centerGrabMove;
        private float _centerGrabAngle;
        private bool _prevGrabJump;

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
            _centerGrabRotateAxis = Vector3.Slerp(Vector3.forward, Vector3.down, _rotateDirection);
        }

        public override void PostLateUpdate()
        {
            _originPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).position;
            _originRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).rotation;
            _leftHandPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
            _rightHandPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            _centerGrabMove = GetCenterGrabMove();
            _centerGrabAngle = GetCenterGrabAngle();

            _flagGrabBoth = false;
        }

        public override void InputGrab(bool value, UdonInputEventArgs args)
        {
            switch (args.handType)
            {
                case HandType.RIGHT: _isGrabRight = value; break;
                case HandType.LEFT: _isGrabLeft = value; break;
            }
            if (_isGrabBoth = _isGrabLeft & _isGrabRight) { _flagGrabBoth = true; }
        }

        protected override void InputKey()
        {
            InputGrabJump();

            _brakes = _centerGrabMove.z < -_brakesAcceptanceThreshold;
            //_turbo = 

            _thrust = _centerGrabMove.z;

            var angle = -Mathf.Clamp(_centerGrabAngle, -90.0f, 90.0f);
            var directRotation = new Vector3
            (
                _centerGrabMove.y * 90.0f,
                angle,
                angle
            );
            driver._InputDirectRotate(Vector3.Scale(_rotateSign, directRotation));

            angle = Mathf.Clamp(_centerGrabAngle / _rotateRatio, -1.0f, 1.0f);
            _elevator = _centerGrabMove.y;
            _ladder = angle;
            _aileron = angle;
        }

        private Vector3 GetCenterGrabMove()
        {
            var result = Vector3.zero;
            if (!_isGrabBoth) { return result; }

            var avatarScale = Mathf.Max(_localPlayer.GetAvatarEyeHeightAsMeters(), 0.1f);
            var centerHandPosition = (_leftHandPosition + _rightHandPosition) / 2;
            centerHandPosition = Quaternion.Inverse(_originRotation) * (centerHandPosition - _originPosition);

            if (_flagGrabBoth)
            {
                _centerGrabPosition = centerHandPosition;
            }
            result = centerHandPosition - _centerGrabPosition;
            result = result / (avatarScale * _moveScale);

            return result;
        }

        private float GetCenterGrabAngle()
        {
            if (!_isGrabBoth) { return 0.0f; }

            var centerHandDirection = _rightHandPosition - _leftHandPosition;
            centerHandDirection = Quaternion.Inverse(_originRotation) * centerHandDirection;

            if (_flagGrabBoth)
            {
                _centerGrabDirection = centerHandDirection;
            }
            var angle = Vector3.SignedAngle(_centerGrabDirection, centerHandDirection, _centerGrabRotateAxis);

            return angle;
        }

        private void InputGrabJump()
        {
            var grabJump = _centerGrabMove.y > _jumpAcceptanceThreshold;
            if (grabJump && !_prevGrabJump)
            {
                driver._InputJump();
            }
            _prevGrabJump = grabJump;
        }
    }
}
