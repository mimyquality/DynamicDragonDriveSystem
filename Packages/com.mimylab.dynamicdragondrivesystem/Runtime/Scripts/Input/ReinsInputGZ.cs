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
    [DefaultExecutionOrder(-100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsInputGZ : ReinsInputManager
    {
        [Range(0.0f, 1.0f), SerializeField]
        private float _brakesAcceptanceThreshold = 0.9f;

        private VRCPlayerApi _localPlayer;

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
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
            var playerRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).rotation;
            if (playerRotation.w == 0.0f && playerRotation.z == 0.0f && playerRotation.y == 0.0f && playerRotation.x == 0.0f) { playerRotation = Quaternion.identity; }
            var headRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            if (headRotation.w == 0.0f && headRotation.z == 0.0f && headRotation.y == 0.0f && headRotation.x == 0.0f) { headRotation = Quaternion.identity; }
            driver._InputRotateDirect(Quaternion.Inverse(playerRotation) * headRotation);
        }
    }
}
