/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
    public enum ReinsSelectSwitchReinsType
    {
        KeyBoard,
        Thumbsticks,
        VRHands,
        TouchPad
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsSelectSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        DragonReins selecter;

        [SerializeField]
        ReinsSelectSwitchReinsType switchType = default;

        public override void Interact()
        {
            switch (switchType)
            {
                case ReinsSelectSwitchReinsType.KeyBoard: selecter.SetKeyboard(); break;
                case ReinsSelectSwitchReinsType.Thumbsticks: selecter.SetThumbsticks(); break;
                case ReinsSelectSwitchReinsType.VRHands: selecter.SetVRHands(); break;
                case ReinsSelectSwitchReinsType.TouchPad: selecter.SetTouchPad(); break;
            }
        }
    }
}
