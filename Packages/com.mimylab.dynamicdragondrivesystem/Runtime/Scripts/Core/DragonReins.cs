/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
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
        Count
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

        [Space]
        [SerializeField]
        private bool _debugMode = false;

        internal DragonDriver driver;

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

        private DragonReinsInputType _selectedInput = DragonReinsInputType.None;
        public DragonReinsInputType SelectedInput
        {
            get => _selectedInput;
            set
            {
                var selectable = DragonReinsChangeableInputType.None;
                switch (value)
                {
                    case DragonReinsInputType.Thumbsticks: selectable = DragonReinsChangeableInputType.Thumbsticks; break;
                    case DragonReinsInputType.Keyboard: selectable = DragonReinsChangeableInputType.Keyboard; break;
                    case DragonReinsInputType.VRHands: selectable = DragonReinsChangeableInputType.VRHands; break;
                    case DragonReinsInputType.VRHands2: selectable = DragonReinsChangeableInputType.VRHands2; break;
                    case DragonReinsInputType.Gaze: selectable = DragonReinsChangeableInputType.Gaze; break;
                    case DragonReinsInputType.Legacy: selectable = DragonReinsChangeableInputType.Legacy; break;
                }
                if ((int)selectable != (_changeableInput & (int)selectable)) { return; }
                
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

            if (_debugMode)
            {
                _changeableInput = int.MaxValue;
            }

            InputEnabled = InputEnabled;
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

        public void SetChangeableInput(DragonReinsInputType inputType)
        {
            switch (inputType)
            {
                case DragonReinsInputType.Thumbsticks:
                    if (thumbsticks) { _changeableInput |= (int)DragonReinsChangeableInputType.Thumbsticks; }
                    break;
                case DragonReinsInputType.Keyboard:
                    if (keyboard) { _changeableInput |= (int)DragonReinsChangeableInputType.Keyboard; }
                    break;
                case DragonReinsInputType.VRHands:
                    if (vrHands) { _changeableInput |= (int)DragonReinsChangeableInputType.VRHands; }
                    break;
                case DragonReinsInputType.VRHands2:
                    if (vrHands2) { _changeableInput |= (int)DragonReinsChangeableInputType.VRHands2; }
                    break;
                case DragonReinsInputType.Gaze:
                    if (gaze) { _changeableInput |= (int)DragonReinsChangeableInputType.Gaze; }
                    break;
                case DragonReinsInputType.Legacy:
                    if (legacy) { _changeableInput |= (int)DragonReinsChangeableInputType.Legacy; }
                    break;
            }
        }
    }
}
