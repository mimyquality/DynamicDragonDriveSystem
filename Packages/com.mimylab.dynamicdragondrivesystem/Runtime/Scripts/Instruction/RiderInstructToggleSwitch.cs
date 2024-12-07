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
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider ToggleSwitch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructToggleSwitch : RiderInstructSwitchBase
    {
        internal RiderInstructToggle toggler;
        internal bool isOn;

        public override void Interact()
        {
            if (toggler) { toggler._Change(isOn); }
        }
    }
}
