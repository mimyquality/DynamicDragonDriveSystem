/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace MimyLab.DynamicDragonDriveSystem
{
    public enum DragonReinsInputType
    {
        Keyboard,
        Thumbsticks,
        VRHands
    }

    [DefaultExecutionOrder(-200)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonReins : UdonSharpBehaviour
    {
        public DragonDriver driver;

        public ReinsInputKB keyboard;
        public ReinsInputSTK thumbsticks;
        public ReinsInputVR vrHands;
        //public ReinsInputTP touchPad;

        [FieldChangeCallback(nameof(EnableReinsInput))]
        private bool _enableReinsInput = false;
        public bool EnableReinsInput
        {
            get => _enableReinsInput;
            set
            {
                _enableReinsInput = value;

                keyboard.enabled = (SelectedInput == DragonReinsInputType.Keyboard) && value;
                thumbsticks.enabled = (SelectedInput == DragonReinsInputType.Thumbsticks) && value;
                vrHands.enabled = (SelectedInput == DragonReinsInputType.VRHands) && value;
            }
        }

        [FieldChangeCallback(nameof(SelectedInput))]
        private DragonReinsInputType _selectedInput = default;
        public DragonReinsInputType SelectedInput
        {
            get => _selectedInput;
            set
            {
                _selectedInput = value;

                //Debug
                selectedKeyboard.SetActive(value == DragonReinsInputType.Keyboard);
                selectedThumbsticks.SetActive(value == DragonReinsInputType.Thumbsticks);
                selectedVRHands.SetActive(value == DragonReinsInputType.VRHands);
            }
        }

        [SerializeField] private GameObject selectedKeyboard;
        [SerializeField] private GameObject selectedThumbsticks;
        [SerializeField] private GameObject selectedVRHands;
        //[SerializeField] private GameObject selectedTouchPad;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            keyboard = GetComponentInChildren<ReinsInputKB>(true);
            thumbsticks = GetComponentInChildren<ReinsInputSTK>(true);
            vrHands = GetComponentInChildren<ReinsInputVR>(true);
            //touchPad = GetComponentInChildren<ReinsInputTP>();

            keyboard.driver = driver;
            thumbsticks.driver = driver;
            vrHands.driver = driver;
            // touchPad初期化

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            if (Networking.LocalPlayer.IsUserInVR())
            {
                SelectedInput = DragonReinsInputType.Thumbsticks;
            }
            else
            {
                SelectedInput = DragonReinsInputType.Keyboard;
            }
            EnableReinsInput = EnableReinsInput;
        }

        public void SetKeyboard()
        {
            SelectedInput = DragonReinsInputType.Keyboard;
        }

        public void SetThumbsticks()
        {
            SelectedInput = DragonReinsInputType.Thumbsticks;
        }

        public void SetVRHands()
        {
            SelectedInput = DragonReinsInputType.VRHands;
        }

        public void SetTouchPad() { }
    }
}
