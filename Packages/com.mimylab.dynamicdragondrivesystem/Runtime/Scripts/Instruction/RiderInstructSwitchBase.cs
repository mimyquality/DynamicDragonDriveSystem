/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using UnityEngine.UI;
    //using VRC.SDKBase;
    //using VRC.Udon;

    [Icon(ComponentIconPath.DDDSystem)]
    [RequireComponent(typeof(Button))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructSwitchBase : UdonSharpBehaviour
    {
        protected Button _button = null;

        internal void _SetInteractable(bool value)
        {
            if (!_button) { _button = GetComponent<Button>(); }
            if (_button) { _button.interactable = value; }
        }
    }
}
