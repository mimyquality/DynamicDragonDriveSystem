/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider ToggleSwitch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructToggleSwitch : RiderInstructSwitchBase
    {
        internal RiderInstructToggle _toggler;
        internal bool _isOn;

        public override void Interact()
        {
            if (_toggler) { _toggler._Change(_isOn); }
        }
    }
}
