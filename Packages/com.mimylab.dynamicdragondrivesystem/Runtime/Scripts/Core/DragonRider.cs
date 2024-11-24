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
    using VRC.Udon.Common;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Rider")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonRider : UdonSharpBehaviour
    {
        internal DragonDriver driver;
        internal DragonSaddle saddle;
        internal DragonReins reins;
        internal DragonActor actor;

        private DragonBonds _bonds;

        // Saddle
        private Vector3 _seatPosition = Vector3.zero;

        // Seat
        private bool _isRide = false;
        private bool _isMount = false;
        private int _mountCount = 0;

        // Reins
        private DragonReinsInputType _selectedInput = DragonReinsInputType.None;
        private bool[] _throttleInputHand = new bool[6];
        private bool[] _turningInputHand = new bool[6];
        private bool[] _elevatorInputHand = new bool[6];
        private bool[] _invertThrust = new bool[6];
        private bool[] _invertClimb = new bool[6];
        private bool[] _invertStrafe = new bool[6];
        private bool[] _invertElevator = new bool[6];
        private bool[] _invertLadder = new bool[6];
        private bool[] _invertAileron = new bool[6];
        private bool[] _vrGrabMode = new bool[2];

        // Canopy
        private bool _canopyIndication = false;

        public bool IsRide { get => _isRide; }
        public bool IsMount { get => _isMount; }

        /* 
        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        } */

        public void _OnRideSaddle()
        {
            Networking.SetOwner(Networking.LocalPlayer, driver.gameObject);
            if (actor) { Networking.SetOwner(Networking.LocalPlayer, actor.gameObject); }
            driver.IsDrive = true;
            saddle.EnabledAdjust = false;
            reins.InputEnabled = true;
        }

        public void _OnExitSaddle()
        {
            driver.IsDrive = false;
            saddle.EnabledAdjust = false;
            reins.InputEnabled = false;
        }

        public void _OnRemind(DragonBonds bonds)
        {

        }

        public void _Bond(DragonBonds bonds)
        {
            _bonds = bonds;
        }
        
        internal void _SetIsRide(bool value)
        {
            _isRide = value;
        }

        internal void _SetIsMount(bool value)
        {
            _mountCount += value ? 1 : -1;
            _mountCount = Mathf.Max(_mountCount, 0);

            _isMount = _mountCount > 0;
        }
    }
}
