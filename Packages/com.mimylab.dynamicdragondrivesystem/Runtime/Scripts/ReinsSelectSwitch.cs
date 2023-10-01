
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
        Joysticks,
        VRHand,
        TouchPad
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsSelectSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        DragonReinsSelecter selecter;

        [SerializeField]
        ReinsSelectSwitchReinsType switchType = default;

        public override void Interact()
        {
            switch (switchType)
            {
                case ReinsSelectSwitchReinsType.KeyBoard: selecter.SetKeyboard(); break;
                case ReinsSelectSwitchReinsType.Joysticks: selecter.SetJoysticks(); break;
                case ReinsSelectSwitchReinsType.VRHand: selecter.SetVRHands(); break;
                case ReinsSelectSwitchReinsType.TouchPad: selecter.SetTouchPad(); break;
            }
        }
    }
}
