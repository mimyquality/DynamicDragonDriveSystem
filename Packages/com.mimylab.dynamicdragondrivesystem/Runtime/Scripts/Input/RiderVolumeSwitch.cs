/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
    //using VRC.Udon;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Input/Rider VolumeSwitch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderVolumeSwitch : RiderUISwitchManager
    {
        internal RiderVolume volumer;

        [SerializeField]
        private float _volume = 0.0f;

        public override void Interact()
        {
            if (volumer) { volumer._Change(_volume); }
        }
    }
}
