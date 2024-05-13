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

        [SerializeField]
        private Button _button_Keyboard;
        [SerializeField]
        private Button _button_Thumbsticks;
        [SerializeField]
        private Button _button_VRHands;
        [SerializeField]
        private Button _button_Gaze;
        [SerializeField]
        private Button _button_Legacy;

        [SerializeField]
        private GameObject _menu_Keyboard;
        [SerializeField]
        private GameObject _menu_Thumbsticks;
        [SerializeField]
        private GameObject _menu_VRHands;
        [SerializeField]
        private GameObject _menu_Gaze;
        [SerializeField]
        private GameObject _menu_Legacy;

        private void OnEnable()
        {
            SetChangedInput(reins.ChangedInput);
            SetSelectInput(reins.SelectedImput);
        }

        public override void OnInputMethodChanged(VRCInputMethod inputMethod)
        {
            switch (inputMethod)
            {
                case VRCInputMethod.Keyboard:
                    _button_Keyboard.gameObject.SetActive(true);
                    break;
                case VRCInputMethod.Mouse:
                case VRCInputMethod.Touch:
                    _button_Gaze.gameObject.SetActive(true);
                    break;
                case VRCInputMethod.Controller:
                    _button_Thumbsticks.gameObject.SetActive(true);
                    break;
                case VRCInputMethod.Vive:
                    _button_Legacy.gameObject.SetActive(false);
                    break;
                case VRCInputMethod.Oculus:
                case VRCInputMethod.ViveXr:
                case VRCInputMethod.Index:
                case VRCInputMethod.HPMotionController:
                case VRCInputMethod.QuestHands:
                case VRCInputMethod.OpenXRGeneric:
                case VRCInputMethod.Pico:
                    _button_VRHands.gameObject.SetActive(true);
                    break;
                default:
                    _button_Thumbsticks.gameObject.SetActive(true);
                    break;
            }
        }

        public void _SetKeyboard()
        {
            SetSelectInput(DragonReinsInputType.Keyboard);
        }

        public void _SetThumbsticks()
        {
            SetSelectInput(DragonReinsInputType.Thumbsticks);
        }

        public void _SetVRHands()
        {
            SetSelectInput(DragonReinsInputType.VRHands);
        }

        public void _SetGaze()
        {
            SetSelectInput(DragonReinsInputType.Gaze);
        }

        public void _SetLegacy()
        {
            SetSelectInput(DragonReinsInputType.Legacy);
        }

        private void SetSelectInput(DragonReinsInputType selectInput)
        {
            _button_Keyboard.interactable = selectInput != DragonReinsInputType.Keyboard;
            _button_Thumbsticks.interactable = selectInput != DragonReinsInputType.Thumbsticks;
            _button_VRHands.interactable = selectInput != DragonReinsInputType.VRHands;
            _button_Gaze.interactable = selectInput != DragonReinsInputType.Gaze;
            _button_Legacy.interactable = selectInput != DragonReinsInputType.Legacy;

            _menu_Keyboard.SetActive(selectInput == DragonReinsInputType.Keyboard);
            _menu_Thumbsticks.SetActive(selectInput == DragonReinsInputType.Thumbsticks);
            _menu_VRHands.SetActive(selectInput == DragonReinsInputType.VRHands);
            _menu_Gaze.SetActive(selectInput == DragonReinsInputType.Gaze);
            _menu_Legacy.SetActive(selectInput == DragonReinsInputType.Legacy);
        }

        private void SetChangedInput(int changedInput)
        {
            _button_Keyboard.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.Keyboard));
            _button_Thumbsticks.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.Thumbsticks));
            _button_VRHands.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.VRHands));
            _button_Gaze.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.Gaze));
            _button_Legacy.gameObject.SetActive(changedInput == (changedInput | (int)DragonReinsInputType.Legacy));
        }
    }
}
