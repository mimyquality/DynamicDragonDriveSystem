/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
    //using VRC.Udon;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider VolumeSwitch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructVolumeSwitch : RiderInstructSwitchBase
    {
        [SerializeField]
        private float _volume = 0.0f;

        internal RiderInstructVolume volumer;

        public override void Interact()
        {
            if (volumer) { volumer._Change(_volume); }
        }
    }
}
