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

    [AddComponentMenu("Dynamic Dragon Drive System/Input/ReinsInput Gaze")]
    public class ReinsInputGZ : ReinsController
    {
        [SerializeField, Range(0.0f, 1.0f)]
        private float _brakesAcceptanceThreshold = 0.9f;

        private VRCPlayerApi _localPlayer;
        private Vector3 _gazeAngles;

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
        }

        public override void PostLateUpdate()
        {
            _gazeAngles = GetGazeAngles();
        }

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            _thrust = value;
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            _brakes = value < -_brakesAcceptanceThreshold;
        }

        protected override void InputKey()
        {
            driver._InputDirectRotate(_gazeAngles, true);
        }

        private Vector3 GetGazeAngles()
        {
            var playerRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).rotation;
            var headRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            var gazeRotation = Quaternion.Inverse(playerRotation) * headRotation;
            if (gazeRotation.w == 0.0f && gazeRotation.z == 0.0f && gazeRotation.y == 0.0f && gazeRotation.x == 0.0f)
            {
                return Vector3.zero;
            }

            var gazeDirection = gazeRotation * Vector3.forward;
            var horizontalDirection = Vector3.ProjectOnPlane(gazeDirection, Vector3.up);
            var yaw = Vector3.SignedAngle(Vector3.forward, horizontalDirection, Vector3.up);
            var pitch = Vector3.SignedAngle(horizontalDirection, gazeDirection, Quaternion.LookRotation(horizontalDirection) * Vector3.right);
            return new Vector3(pitch, yaw);
        }
    }
}
