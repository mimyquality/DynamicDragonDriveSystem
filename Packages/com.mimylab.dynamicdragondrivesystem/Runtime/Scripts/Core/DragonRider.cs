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
    using VRC.Udon;
    using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Rider")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonRider : UdonSharpBehaviour
    {

        private DragonBonds _bonds;

        private DragonReinsInputType[] _selectedInput = new DragonReinsInputType[2];

        // ToDo: 各種操作方法毎の入力値反転設定とか

        private Vector3 _seatPosition = Vector3.zero;
        private bool _showCanopy = false;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }



            _initialized = true;
        }
        private void Start()
        {
            Initialize();


        }

        public void _Remind(DragonBonds bonds)
        {
            _bonds = bonds;
        }
    }
}
