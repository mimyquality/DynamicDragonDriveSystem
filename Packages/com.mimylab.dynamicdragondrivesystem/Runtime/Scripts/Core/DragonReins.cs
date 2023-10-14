/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
//using VRC.Udon;
//using VRC.SDK3.Components;

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
        [HideInInspector]
        public DragonDriver driver;

        public ReinsInputKB keyboard;
        public ReinsInputSTK thumbsticks;
        public ReinsInputVR vrHands;
        //public ReinsInputTP touchPad;

        [FieldChangeCallback(nameof(EnabledInput))]
        private bool _enabledInput = false;
        public bool EnabledInput
        {
            get => _enabledInput;
            set
            {
                _enabledInput = value;

                if (keyboard) keyboard.enabled = (_selectedInput == DragonReinsInputType.Keyboard) && value;
                if (thumbsticks) thumbsticks.enabled = (_selectedInput == DragonReinsInputType.Thumbsticks) && value;
                if (vrHands) vrHands.enabled = (_selectedInput == DragonReinsInputType.VRHands) && value;
            }
        }

        private DragonReinsInputType _selectedInput = default;
        public DragonReinsInputType SelectedImput { get => _selectedInput; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            if (keyboard) keyboard = GetComponentInChildren<ReinsInputKB>(true);
            if (thumbsticks) thumbsticks = GetComponentInChildren<ReinsInputSTK>(true);
            if (vrHands) vrHands = GetComponentInChildren<ReinsInputVR>(true);
            //touchPad = GetComponentInChildren<ReinsInputTP>();

            if (keyboard) keyboard.driver = driver;
            if (thumbsticks) thumbsticks.driver = driver;
            if (vrHands) vrHands.driver = driver;
            // touchPad初期化

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            if (Networking.LocalPlayer.IsUserInVR())
            {
                _selectedInput = DragonReinsInputType.Thumbsticks;
            }
            else
            {
                _selectedInput = DragonReinsInputType.Keyboard;
            }
            EnabledInput = EnabledInput;
        }

        public void _SetKeyboard()
        {
            _selectedInput = DragonReinsInputType.Keyboard;
            EnabledInput = EnabledInput;
        }

        public void _SetThumbsticks()
        {
            _selectedInput = DragonReinsInputType.Thumbsticks;
            EnabledInput = EnabledInput;
        }

        public void _SetVRHands()
        {
            _selectedInput = DragonReinsInputType.VRHands;
            EnabledInput = EnabledInput;
        }

        public void _SetTouchPad() { }

        public void _EnableDragonControl()
        {
            EnabledInput = true;
        }

        public void _DisableDragonControl()
        {
            EnabledInput = false;
        }
    }
}
