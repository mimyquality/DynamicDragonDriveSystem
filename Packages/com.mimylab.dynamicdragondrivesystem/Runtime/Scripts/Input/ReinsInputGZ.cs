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

    [AddComponentMenu("Dynamic Dragon Drive System/ReinsInput Gaze")]
    public class ReinsInputGZ : ReinsInputManager
    {
        [Range(0.0f, 1.0f), SerializeField]
        private float _brakesAcceptanceThreshold = 0.9f;

        private VRCPlayerApi _localPlayer;
        private Quaternion _gazeRotation;

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
        }

        public override void PostLateUpdate()
        {
            if (!this.enabled) { return; }

            var playerRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).rotation;
            var headRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            _gazeRotation = Quaternion.Inverse(playerRotation) * headRotation;
            if (_gazeRotation.w == 0.0f && _gazeRotation.z == 0.0f && _gazeRotation.y == 0.0f && _gazeRotation.x == 0.0f)
            {
                _gazeRotation = Quaternion.identity;
            }
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
            driver._InputGazeRotate(_gazeRotation);
        }
    }
}
