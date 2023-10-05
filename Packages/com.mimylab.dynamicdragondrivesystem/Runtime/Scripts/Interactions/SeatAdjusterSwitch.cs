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
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SeatAdjusterSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        DragonSeat _seat;

        [SerializeField]
        MeshRenderer _lamp;
        [SerializeField]
        Material _enableMaterial, _disableMaterial;

        private void OnEnable()
        {
            _lamp.sharedMaterial = (_seat.EnableSeatAdjust) ? _enableMaterial : _disableMaterial;
        }

        public override void Interact()
        {
            var toggleAdjust = !_seat.EnableSeatAdjust;
            _seat.EnableSeatAdjust = toggleAdjust;
            _lamp.sharedMaterial = (toggleAdjust) ? _enableMaterial : _disableMaterial;
        }
    }
}
