/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    public enum PlatformType
    {
        Unknown,
        VR,
        Desktop,
        Quest,
        Android
    }

    public enum DragonReinsInputType
    {
        Keyboard,
        Thumbsticks,
        VRHands,
        Gaze,
        Legacy
    }

    [AddComponentMenu("Dynamic Dragon Drive System/Dragon Reins")]
    [DefaultExecutionOrder(-200)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonReins : UdonSharpBehaviour
    {
        public ReinsInputKB keyboard;
        public ReinsInputSTK thumbsticks;
        public ReinsInputVR vrHands;
        public ReinsInputGZ gaze;
        public ReinsInputLGC legacy;

        internal DragonDriver driver;

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
                if (gaze) gaze.enabled = (_selectedInput == DragonReinsInputType.Gaze) && value;
                if (legacy) legacy.enabled = (_selectedInput == DragonReinsInputType.Legacy) && value;
            }
        }

        private DragonReinsInputType _selectedInput = default;
        public DragonReinsInputType SelectedImput { get => _selectedInput; }

        private PlatformType _platform = default;
        public PlatformType Platform { get => _platform; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            if (!keyboard) keyboard = GetComponentInChildren<ReinsInputKB>(true);
            if (!thumbsticks) thumbsticks = GetComponentInChildren<ReinsInputSTK>(true);
            if (!vrHands) vrHands = GetComponentInChildren<ReinsInputVR>(true);
            if (!gaze) gaze = GetComponentInChildren<ReinsInputGZ>(true);
            if (!legacy) legacy = GetComponentInChildren<ReinsInputLGC>(true);

            if (keyboard) keyboard.driver = driver;
            if (thumbsticks) thumbsticks.driver = driver;
            if (vrHands) vrHands.driver = driver;
            if (gaze) gaze.driver = driver;
            if (legacy) legacy.driver = driver;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

#if UNITY_STANDALONE_WIN
            _platform = (Networking.LocalPlayer.IsUserInVR()) ? PlatformType.VR : PlatformType.Desktop;
#elif UNITY_ANDROID
            _platform = (Networking.LocalPlayer.IsUserInVR()) ? PlatformType.Quest : PlatformType.Android;
#endif

            switch (_platform)
            {
                case PlatformType.VR: _SetThumbsticks(); break;
                case PlatformType.Desktop: _SetKeyboard(); break;
                case PlatformType.Quest: _SetThumbsticks(); break;
                case PlatformType.Android: _SetGaze(); break;
            }
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

        public void _SetGaze()
        {
            _selectedInput = DragonReinsInputType.Gaze;
            EnabledInput = EnabledInput;
        }

        public void _SetLegacy()
        {
            _selectedInput = DragonReinsInputType.Legacy;
            EnabledInput = EnabledInput;
        }

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
