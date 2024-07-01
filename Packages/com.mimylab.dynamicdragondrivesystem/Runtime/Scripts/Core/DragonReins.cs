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

    public enum DragonReinsInputType
    {
        None = 0,
        Keyboard = 1 << 0,
        Thumbsticks = 1 << 1,
        VRHands = 1 << 2,
        Gaze = 1 << 3,
        Legacy = 1 << 4,
        VRHands2 = 1 << 5,
    }

    [AddComponentMenu("Dynamic Dragon Drive System/Dragon Reins")]
    [DefaultExecutionOrder(-200)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonReins : UdonSharpBehaviour
    {
        public ReinsInputKB keyboard;
        public ReinsInputSTK thumbsticks;
        public ReinsInputVR vrHands;
        public ReinsInputVR2 vrHands2;
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

                bool enableSelect;
                if (keyboard)
                {
                    enableSelect = (_selectedInput == DragonReinsInputType.Keyboard) && value;
                    keyboard.enabled = enableSelect;
                    keyboard.gameObject.SetActive(enableSelect);
                }
                if (thumbsticks)
                {
                    enableSelect = (_selectedInput == DragonReinsInputType.Thumbsticks) && value;
                    thumbsticks.enabled = enableSelect;
                    thumbsticks.gameObject.SetActive(enableSelect);
                }
                if (vrHands)
                {
                    enableSelect = (_selectedInput == DragonReinsInputType.VRHands) && value;
                    vrHands.enabled = enableSelect;
                    vrHands.gameObject.SetActive(enableSelect);
                }
                if (vrHands2)
                {
                    enableSelect = (_selectedInput == DragonReinsInputType.VRHands2) && value;
                    vrHands2.enabled = enableSelect;
                    vrHands2.gameObject.SetActive(enableSelect);
                }
                if (gaze)
                {
                    enableSelect = (_selectedInput == DragonReinsInputType.Gaze) && value;
                    gaze.enabled = enableSelect;
                    gaze.gameObject.SetActive(enableSelect);
                }
                if (legacy)
                {
                    enableSelect = (_selectedInput == DragonReinsInputType.Legacy) && value;
                    legacy.enabled = enableSelect;
                    legacy.gameObject.SetActive(enableSelect);
                }
            }
        }

        private DragonReinsInputType _selectedInput = DragonReinsInputType.Thumbsticks;
        public DragonReinsInputType SelectedImput { get => _selectedInput; }

        private int _changedInput = (int)DragonReinsInputType.Thumbsticks;
        public int ChangedInput { get => _changedInput; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            if (!keyboard) keyboard = GetComponentInChildren<ReinsInputKB>(true);
            if (!thumbsticks) thumbsticks = GetComponentInChildren<ReinsInputSTK>(true);
            if (!vrHands) vrHands = GetComponentInChildren<ReinsInputVR>(true);
            if (!vrHands2) vrHands2 = GetComponentInChildren<ReinsInputVR2>(true);
            if (!gaze) gaze = GetComponentInChildren<ReinsInputGZ>(true);
            if (!legacy) legacy = GetComponentInChildren<ReinsInputLGC>(true);

            if (keyboard) keyboard.driver = driver;
            if (thumbsticks) thumbsticks.driver = driver;
            if (vrHands) vrHands.driver = driver;
            if (vrHands2) vrHands2.driver = driver;
            if (gaze) gaze.driver = driver;
            if (legacy) legacy.driver = driver;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            // OnInputMethodChangedが最初に1回発火しないバグ回避
            SelectPlatform();
            EnabledInput = EnabledInput;
        }

        private bool _initializedInputType = false;
        public override void OnInputMethodChanged(VRCInputMethod inputMethod)
        {
            switch (inputMethod)
            {
                case VRCInputMethod.Keyboard:
                    _changedInput |= (int)DragonReinsInputType.Keyboard;
                    break;
                case VRCInputMethod.Mouse:
                case VRCInputMethod.Touch:
                    _changedInput |= (int)DragonReinsInputType.Gaze;
                    break;
                case VRCInputMethod.Controller:
                    _changedInput |= (int)DragonReinsInputType.Thumbsticks;
                    break;
                case VRCInputMethod.Vive:
                    _changedInput |= (int)DragonReinsInputType.Legacy;
                    _changedInput |= (int)DragonReinsInputType.VRHands;
                    _changedInput |= (int)DragonReinsInputType.VRHands2;
                    break;
                case VRCInputMethod.Oculus:
                case VRCInputMethod.ViveXr:
                case VRCInputMethod.Index:
                case VRCInputMethod.HPMotionController:
                case VRCInputMethod.QuestHands:
                case VRCInputMethod.OpenXRGeneric:
                case VRCInputMethod.Pico:
                    _changedInput |= (int)DragonReinsInputType.Thumbsticks;
                    _changedInput |= (int)DragonReinsInputType.VRHands;
                    _changedInput |= (int)DragonReinsInputType.VRHands2;
                    break;
                default:
                    _changedInput |= (int)DragonReinsInputType.Thumbsticks;
                    break;
            }

            if (!_initializedInputType)
            {
                switch (inputMethod)
                {
                    case VRCInputMethod.Keyboard:
                    case VRCInputMethod.Mouse:
                        _SetKeyboard();
                        break;
                    case VRCInputMethod.Touch:
                        _SetGaze();
                        break;
                    case VRCInputMethod.Vive:
                        _SetLegacy();
                        break;
                    default:
                        _SetThumbsticks();
                        break;
                }
                _initializedInputType = true;
            }
        }

        private void SelectPlatform()
        {
            var isPC = false;
#if UNITY_STANDALONE_WIN
            isPC = true;
#endif
            if (isPC)
            {
                if (Networking.LocalPlayer.IsUserInVR())
                {
                    _changedInput |= (int)DragonReinsInputType.Thumbsticks;
                    _changedInput |= (int)DragonReinsInputType.VRHands;
                    _changedInput |= (int)DragonReinsInputType.VRHands2;
                    _changedInput |= (int)DragonReinsInputType.Legacy;
                    _SetThumbsticks();
                }
                else
                {
                    _changedInput |= (int)DragonReinsInputType.Keyboard;
                    _SetKeyboard();
                }
            }
            else
            {
                if (Networking.LocalPlayer.IsUserInVR())
                {
                    _changedInput |= (int)DragonReinsInputType.Thumbsticks;
                    _changedInput |= (int)DragonReinsInputType.VRHands;
                    _changedInput |= (int)DragonReinsInputType.VRHands2;
                    _SetThumbsticks();
                }
                else
                {
                    _changedInput |= (int)DragonReinsInputType.Gaze;
                    _SetGaze();
                }
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

        public void _SetVRHands2()
        {
            _selectedInput = DragonReinsInputType.VRHands2;
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
