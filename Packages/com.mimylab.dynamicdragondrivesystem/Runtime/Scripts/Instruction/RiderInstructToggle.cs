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
    //using VRC.SDKBase;
    //using VRC.Udon;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider Toggle")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructToggle : UdonSharpBehaviour
    {
        internal DragonRider rider;
        internal DragonReinsInputType targetReinsInput = DragonReinsInputType.None;

        [SerializeField]
        private DragonRiderToggleInstruction _instruction;
        [SerializeField]
        private bool _isOn = false;

        [Space]
        [SerializeField]
        private RiderInstructToggleSwitch _switch_ON;
        [SerializeField]
        private RiderInstructToggleSwitch _switch_OFF;

        private Toggle _uiToggle;
        private bool _currentOn = false;

        public DragonRiderToggleInstruction Instruction { get => _instruction; }
        public bool IsOn { get => _currentOn; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _uiToggle = GetComponent<Toggle>();
            _currentOn = _isOn;
            if (_switch_ON)
            {
                _switch_ON.toggler = this;
                _switch_ON.isOn = false;
            }
            if (_switch_OFF)
            {
                _switch_OFF.toggler = this;
                _switch_OFF.isOn = true;
            }

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            _OnValueChanged(_currentOn);
        }

        public override void Interact()
        {
            _Change(_uiToggle ? _uiToggle.isOn : _isOn);
        }

        public void _Change(bool value)
        {
            Initialize();
            
            _currentOn = value;
            rider._OnToggleChanged(this);
        }

        internal void _OnValueChanged(bool value)
        {
            Initialize();

            _currentOn = value;
            if (_uiToggle) { _uiToggle.SetIsOnWithoutNotify(value); }

            if (_switch_ON) { _switch_ON.gameObject.SetActive(value); }
            if (_switch_OFF) { _switch_OFF.gameObject.SetActive(!value); }
        }
    }
}
