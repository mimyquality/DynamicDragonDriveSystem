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
        private Button _button = null;
        private Button UIButton { get => _button ?? (_button = GetComponent<Button>()); }

        internal bool Interactable
        {
            get => UIButton.interactable;
            set => UIButton.interactable = value;
        }
    }
}
