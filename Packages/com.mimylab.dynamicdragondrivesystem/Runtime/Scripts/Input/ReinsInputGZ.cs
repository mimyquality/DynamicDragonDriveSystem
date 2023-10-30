/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
//using VRC.Udon;
using VRC.Udon.Common;
//using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
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
            var headRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            driver._InputRotateDirect(Quaternion.Inverse(playerRotation) * headRotation);
        }
    }
}
