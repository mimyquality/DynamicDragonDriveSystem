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
    [AddComponentMenu("Dynamic Dragon Drive System/Input/Rider Select")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderSelect : UdonSharpBehaviour
    {
        internal DragonRider rider;

        [SerializeField]
        private DragonRiderSelectType _selectType;
        [SerializeField, Min(0)]
        private int _number = 0;

        [Space]
        [SerializeField]
        private RiderSelectSwitch[] _switches = new RiderSelectSwitch[0];

        private Slider _uiSlider;
        private int _select = 0;

        public DragonRiderSelectType SelectType { get => _selectType; }
        public int Select { get => _select; }


        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _uiSlider = GetComponent<Slider>();
            _select = _number;
            for (int i = 0; i < _switches.Length; i++)
            {
                if (_switches[i])
                {
                    _switches[i].selector = this;
                    _switches[i].number = i;
                }
            }

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            _OnValueChanged(_select);
        }

        public override void Interact()
        {
            _Change(_uiSlider ? (int)_uiSlider.value : _number);
        }

        public void _Change(int value)
        {
            _OnValueChanged(value);
            rider._OnSelectChanged(this);
        }

        internal void _OnValueChanged(int value)
        {
            Initialize();

            _select = value;
            if (_uiSlider) { _uiSlider.SetValueWithoutNotify(value); }

            // 非選択状態のスイッチだけ入力を受け付ける
            for (int i = 0; i < _switches.Length; i++)
            {
                if (_switches[i])
                {
                    _switches[i].Interactable = i != value;
                }
            }
        }
    }
}
