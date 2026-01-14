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
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider SelectSwitch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructSelectSwitch : RiderInstructSwitchBase
    {
        internal RiderInstructSelect _selector;
        internal int _number;

        public override void Interact()
        {
            if (_selector) { _selector._Change(_number); }
        }
    }
}
