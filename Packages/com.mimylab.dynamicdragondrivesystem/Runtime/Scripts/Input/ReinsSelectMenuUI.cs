/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using UnityEngine.UI;
    using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [AddComponentMenu("Dynamic Dragon Drive System/Reins SelectMenu UI")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsSelectMenuUI : UdonSharpBehaviour
    {
        public DragonReins reins;

        [Space]
        [SerializeField] private Button _button_Keyboard;
        [SerializeField] private Button _button_Thumbsticks;
        [SerializeField] private Button _button_VRHands;
        [SerializeField] private Button _button_VRHands2;
        [SerializeField] private Button _button_Gaze;
        [SerializeField] private Button _button_Legacy;

        [SerializeField] private GameObject _menu_Keyboard;
        [SerializeField] private GameObject _menu_Thumbsticks;
        [SerializeField] private GameObject _menu_VRHands;
        [SerializeField] private GameObject _menu_VRHands2;
        [SerializeField] private GameObject _menu_Gaze;
        [SerializeField] private GameObject _menu_Legacy;

        private void OnEnable()
        {
            SetChangedInput(reins.ChangedInput);
            SetSelectInput(reins.SelectedInput);
        }

        public override void OnInputMethodChanged(VRCInputMethod inputMethod)
        {
            SetChangedInput(reins.ChangedInput);
        }

        public void _SetKeyboard()
        {
            reins._SetKeyboard();
            SetSelectInput(DragonReinsInputType.Keyboard);
        }

        public void _SetThumbsticks()
        {
            reins._SetThumbsticks();
            SetSelectInput(DragonReinsInputType.Thumbsticks);
        }

        public void _SetVRHands()
        {
            reins._SetVRHands();
            SetSelectInput(DragonReinsInputType.VRHands);
        }

        public void _SetVRHands2()
        {
            reins._SetVRHands2();
            SetSelectInput(DragonReinsInputType.VRHands2);
        }


        public void _SetGaze()
        {
            reins._SetGaze();
            SetSelectInput(DragonReinsInputType.Gaze);
        }

        public void _SetLegacy()
        {
            reins._SetLegacy();
            SetSelectInput(DragonReinsInputType.Legacy);
        }

        private void SetSelectInput(DragonReinsInputType selectInput)
        {
            if (_button_Keyboard) { _button_Keyboard.interactable = selectInput != DragonReinsInputType.Keyboard; }
            if (_button_Thumbsticks) { _button_Thumbsticks.interactable = selectInput != DragonReinsInputType.Thumbsticks; }
            if (_button_VRHands) { _button_VRHands.interactable = selectInput != DragonReinsInputType.VRHands; }
            if (_button_VRHands2) { _button_VRHands2.interactable = selectInput != DragonReinsInputType.VRHands2; }
            if (_button_Gaze) { _button_Gaze.interactable = selectInput != DragonReinsInputType.Gaze; }
            if (_button_Legacy) { _button_Legacy.interactable = selectInput != DragonReinsInputType.Legacy; }

            if (_menu_Keyboard) { _menu_Keyboard.SetActive(selectInput == DragonReinsInputType.Keyboard); }
            if (_menu_Thumbsticks) { _menu_Thumbsticks.SetActive(selectInput == DragonReinsInputType.Thumbsticks); }
            if (_menu_VRHands) { _menu_VRHands.SetActive(selectInput == DragonReinsInputType.VRHands); }
            if (_menu_VRHands2) { _menu_VRHands2.SetActive(selectInput == DragonReinsInputType.VRHands2); }
            if (_menu_Gaze) { _menu_Gaze.SetActive(selectInput == DragonReinsInputType.Gaze); }
            if (_menu_Legacy) { _menu_Legacy.SetActive(selectInput == DragonReinsInputType.Legacy); }
        }

        private void SetChangedInput(int changedInput)
        {
            if (_button_Keyboard) { _button_Keyboard.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.Keyboard)); }
            if (_button_Thumbsticks) { _button_Thumbsticks.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.Thumbsticks)); }
            if (_button_VRHands) { _button_VRHands.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.VRHands)); }
            if (_button_VRHands2) { _button_VRHands2.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.VRHands2)); }
            if (_button_Gaze) { _button_Gaze.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.Gaze)); }
            if (_button_Legacy) { _button_Legacy.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.Legacy)); }
        }
    }
}
