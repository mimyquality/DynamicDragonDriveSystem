/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    //using VRC.Udon;

    public enum DragonReinsInputType
    {
        None = -1,
        Thumbsticks,
        Keyboard,
        VRHands,
        VRHands2,
        Gaze,
        Legacy,
        count
    }

    public enum DragonReinsChangeableInputType
    {
        None = 0,
        Thumbsticks = 1 << DragonReinsInputType.Thumbsticks,
        Keyboard = 1 << DragonReinsInputType.Keyboard,
        VRHands = 1 << DragonReinsInputType.VRHands,
        VRHands2 = 1 << DragonReinsInputType.VRHands2,
        Gaze = 1 << DragonReinsInputType.Gaze,
        Legacy = 1 << DragonReinsInputType.Legacy,
    }

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Reins")]
    [DefaultExecutionOrder(-20)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonReins : UdonSharpBehaviour
    {
        public ReinsInputSTK thumbsticks;
        public ReinsInputKB keyboard;
        public ReinsInputVR vrHands;
        public ReinsInputVR2 vrHands2;
        public ReinsInputGZ gaze;
        public ReinsInputLGC legacy;

        internal DragonDriver driver;

        [Space]
        [SerializeField]
        private bool _debugMode = false;

        [FieldChangeCallback(nameof(InputEnabled))]
        private bool _inputEnabled = false;
        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                Initialize();

                bool enableSelect;
                if (thumbsticks)
                {
                    enableSelect = (_selectedInput == DragonReinsInputType.Thumbsticks) && value;
                    thumbsticks.enabled = enableSelect;
                    thumbsticks.gameObject.SetActive(enableSelect);
                }
                if (keyboard)
                {
                    enableSelect = (_selectedInput == DragonReinsInputType.Keyboard) && value;
                    keyboard.enabled = enableSelect;
                    keyboard.gameObject.SetActive(enableSelect);
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

                _inputEnabled = value;
            }
        }

        [FieldChangeCallback(nameof(SelectedInput))]
        private DragonReinsInputType _selectedInput = DragonReinsInputType.None;
        public DragonReinsInputType SelectedInput
        {
            get => _selectedInput;
            set
            {
                _selectedInput = value;
                InputEnabled = InputEnabled;
            }
        }

        private int _changeableInput = 0;
        public int ChangeableInput { get => _changeableInput; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

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

            _changeableInput |= SetChangeableInputFromPlatform();
            InputEnabled = InputEnabled;
        }

        private bool _isFirstInputMethodChanged = true;
        public override void OnInputMethodChanged(VRCInputMethod inputMethod)
        {
            if (_debugMode)
            {
                _changeableInput = 0xFF;
            }

            int inputMethodInt = (int)inputMethod;
            switch (inputMethodInt)
            {
                case (int)VRCInputMethod.Keyboard:
                case (int)VRCInputMethod.Mouse:
                    if (keyboard)
                    {
                        _changeableInput |= (int)DragonReinsChangeableInputType.Keyboard;
                        if (_isFirstInputMethodChanged) { _selectedInput = DragonReinsInputType.Keyboard; }
                    }
                    if (gaze) { _changeableInput |= (int)DragonReinsChangeableInputType.Gaze; }
                    break;
                case (int)VRCInputMethod.Controller:
                    if (thumbsticks)
                    {
                        _changeableInput |= (int)DragonReinsChangeableInputType.Thumbsticks;
                        if (_isFirstInputMethodChanged) { _selectedInput = DragonReinsInputType.Thumbsticks; }
                    }
                    break;
                case (int)VRCInputMethod.Vive:
                    if (legacy)
                    {
                        _changeableInput |= (int)DragonReinsChangeableInputType.Legacy;
                        if (_isFirstInputMethodChanged) { _selectedInput = DragonReinsInputType.Legacy; }
                    }
                    if (vrHands) { _changeableInput |= (int)DragonReinsChangeableInputType.VRHands; }
                    if (vrHands2) { _changeableInput |= (int)DragonReinsChangeableInputType.VRHands2; }
                    break;
                case (int)VRCInputMethod.Oculus:
                case (int)VRCInputMethod.ViveXr:
                case (int)VRCInputMethod.Index:
                case (int)VRCInputMethod.HPMotionController:
                case (int)VRCInputMethod.QuestHands:
                case (int)VRCInputMethod.OpenXRGeneric:
                case (int)VRCInputMethod.Pico:
                case (int)VRCInputMethod.SteamVR2:
                    if (thumbsticks)
                    {
                        _changeableInput |= (int)DragonReinsChangeableInputType.Thumbsticks;
                        if (_isFirstInputMethodChanged) { _selectedInput = DragonReinsInputType.Thumbsticks; }
                    }
                    if (vrHands) { _changeableInput |= (int)DragonReinsChangeableInputType.VRHands; }
                    if (vrHands2) { _changeableInput |= (int)DragonReinsChangeableInputType.VRHands2; }
                    break;
                case (int)VRCInputMethod.Touch:
                    if (gaze)
                    {
                        _changeableInput |= (int)DragonReinsChangeableInputType.Gaze;
                        if (_isFirstInputMethodChanged) { _selectedInput = DragonReinsInputType.Gaze; }
                    }
                    break;
                default:
                    if (thumbsticks)
                    {
                        _changeableInput |= (int)DragonReinsChangeableInputType.Thumbsticks;
                        if (_isFirstInputMethodChanged) { _selectedInput = DragonReinsInputType.Thumbsticks; }
                    }
                    break;
            }

            _isFirstInputMethodChanged = false;
        }

        public ReinsInputManager _GetInput(DragonReinsInputType inputType)
        {
            switch (inputType)
            {
                case DragonReinsInputType.Thumbsticks: return thumbsticks;
                case DragonReinsInputType.Keyboard: return keyboard;
                case DragonReinsInputType.VRHands: return vrHands;
                case DragonReinsInputType.VRHands2: return vrHands2;
                case DragonReinsInputType.Gaze: return gaze;
                case DragonReinsInputType.Legacy: return legacy;
                default: return null;
            }
        }

        public ReinsInputManager _GetEnabledInput()
        {
            if (!InputEnabled) { return null; }

            switch (_selectedInput)
            {
                case DragonReinsInputType.Thumbsticks: return thumbsticks;
                case DragonReinsInputType.Keyboard: return keyboard;
                case DragonReinsInputType.VRHands: return vrHands;
                case DragonReinsInputType.VRHands2: return vrHands2;
                case DragonReinsInputType.Gaze: return gaze;
                case DragonReinsInputType.Legacy: return legacy;
                default: return null;
            }
        }

        private int SetChangeableInputFromPlatform()
        {
            var result = 0;

            // General
            if (thumbsticks) { result |= (int)DragonReinsChangeableInputType.Thumbsticks; }

            if (Networking.LocalPlayer.IsUserInVR())
            {
                // VR
                if (vrHands) { result |= (int)DragonReinsChangeableInputType.VRHands; }
                if (vrHands2) { result |= (int)DragonReinsChangeableInputType.VRHands2; }
            }
            else
            {
                // Desktop, Mobile
                if (gaze) { result |= (int)DragonReinsChangeableInputType.Gaze; }
            }

            return result;
        }
    }
}
