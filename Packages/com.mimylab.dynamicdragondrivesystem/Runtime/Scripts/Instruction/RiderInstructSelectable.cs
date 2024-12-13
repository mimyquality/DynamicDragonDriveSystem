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


    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider Selectable")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderInstructSelectable : UdonSharpBehaviour
    {
        [SerializeField]
        private DragonRiderSelectInstruction _instruction;

        [Space]
        [SerializeField]
        private RiderInstructSelectSwitch[] _switches = new RiderInstructSelectSwitch[0];
        internal DragonRider rider;

        private int _selectable = 0;

        public DragonRiderSelectInstruction Instruction { get => _instruction; }
        public int Selectable { get => _selectable; }

        private void Start()
        {
            _OnValueChanged(_selectable);
        }

        internal void _OnValueChanged(int value)
        {
            _selectable = value;

            // 選択可能なスイッチだけアクティブにする
            for (int i = 0; i < _switches.Length; i++)
            {
                if (_switches[i])
                {
                    var flag = _selectable == (_selectable | (1 << i));
                    _switches[i].gameObject.SetActive(flag);
                }
            }
        }
    }
}
