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
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider Volume")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructVolume : UdonSharpBehaviour
    {
        internal DragonRider rider;        
        internal DragonReinsInputType targetReinsInput = DragonReinsInputType.None;

        [SerializeField]
        private DragonRiderVolumeInstruction _instruction;
        [SerializeField]
        private float _volume = 0.0f;

        [Space]
        [SerializeField]
        private RiderInstructVolumeSwitch[] _switches = new RiderInstructVolumeSwitch[0];

        private Slider _uiSlider;
        private float _currentVolume = 0.0f;

        public DragonRiderVolumeInstruction Instruction { get => _instruction; }
        public float Volume { get => _currentVolume; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _uiSlider = GetComponent<Slider>();
            _currentVolume = _volume;

            for (int i = 0; i < _switches.Length; i++)
            {
                if (_switches[i])
                {
                    _switches[i].volumer = this;
                }
            }

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            _OnValueChanged(_currentVolume);
        }

        public override void Interact()
        {
            _Change(_uiSlider ? _uiSlider.value : _volume);
        }

        public void _Change(float value)
        {
            _currentVolume = value;
            rider._OnVolumeChanged(this);
        }

        internal void _OnValueChanged(float value)
        {
            Initialize();

            _currentVolume = value;
            if (_uiSlider) { _uiSlider.SetValueWithoutNotify(value); }
        }
    }
}
