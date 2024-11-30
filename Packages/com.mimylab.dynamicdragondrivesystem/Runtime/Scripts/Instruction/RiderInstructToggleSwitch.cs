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
    [AddComponentMenu("Dynamic Dragon Drive System/Input/Rider ToggleSwitch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructToggleSwitch : RiderInstructSwitchBase
    {
        internal RiderInstructToggle toggler;

        [SerializeField]
        private bool _isOn;

        public override void Interact()
        {
            if (toggler) { toggler._Change(_isOn); }
        }
    }
}
