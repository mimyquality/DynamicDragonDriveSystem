﻿/*
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
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider SelectSwitch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructSelectSwitch : RiderInstructSwitchBase
    {
        internal RiderInstructSelect selector;

        [SerializeField]
        private int _number;

        public override void Interact()
        {
            if (selector) { selector._Change(_number); }
        }
    }
}
