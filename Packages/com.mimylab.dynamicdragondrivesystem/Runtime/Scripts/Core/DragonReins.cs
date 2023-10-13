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

        [FieldChangeCallback(nameof(IsEnabledInput))]
        private bool _isEnabledInput = false;
        public bool IsEnabledInput
        {
            get => _isEnabledInput;
            set
            {
                _isEnabledInput = value;

                keyboard.enabled = (_selectedInput == DragonReinsInputType.Keyboard) && value;
                thumbsticks.enabled = (_selectedInput == DragonReinsInputType.Thumbsticks) && value;
                vrHands.enabled = (_selectedInput == DragonReinsInputType.VRHands) && value;
            }
        }

        private DragonReinsInputType _selectedInput = default;
        private DragonReinsInputType SelectedInput
        {
            get => _selectedInput;
            set => _selectedInput = value;
        }

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
            IsEnabledInput = IsEnabledInput;
        }

        public void _SetKeyboard()
        {
            SelectedInput = DragonReinsInputType.Keyboard;
            IsEnabledInput = IsEnabledInput;
        }

        public void _SetThumbsticks()
        {
            SelectedInput = DragonReinsInputType.Thumbsticks;
            IsEnabledInput = IsEnabledInput;
        }

        public void _SetVRHands()
        {
            SelectedInput = DragonReinsInputType.VRHands;
            IsEnabledInput = IsEnabledInput;
        }

        public void _SetTouchPad() { }

        public void _EnableDragonControl()
        {
            IsEnabledInput = true;
        }

        public void _DisableDragonControl()
        {
            IsEnabledInput = false;
        }
    }
}
