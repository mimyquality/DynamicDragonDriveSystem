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

    public enum DragonRiderToggleType
    {
        SeatAdjust,
        InvertThrust,
        InvertClimb,
        InvertStrafe,
        InvertElevator,
        InvertLadder,
        InvertAileron,
        VRGrabMode,
        CanopyIndication,
    }

    public enum DragonRiderSelectType
    {
        ReinsInput,
        ThrottleInputHand,
        TurningInputHand,
        ElevatorInputHand,
    }

    public enum DragonRiderVolumeType
    {
        None
    }

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
        private RiderToggle[] _toggles = new RiderToggle[0];
        private RiderSelect[] _selects = new RiderSelect[0];
        private RiderVolume[] _volumes = new RiderVolume[0];

        // Saddle
        private Vector3 _seatPosition = Vector3.zero;

        // Seat
        private bool _isRide = false;
        private bool _isMount = false;
        private int _mountCount = 0;

        // Reins
        private DragonReinsInputType _selectedInput = default;
        private byte[] _throttleInputHand = new byte[6];
        private byte[] _turningInputHand = new byte[6];
        private byte[] _elevatorInputHand = new byte[6];
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

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _toggles = GetComponentsInChildren<RiderToggle>(true);
            _selects = GetComponentsInChildren<RiderSelect>(true);
            _volumes = GetComponentsInChildren<RiderVolume>(true);

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        /******************************
         Saddle からのイベント
         ******************************/
        internal void _OnRideSaddle()
        {
            Networking.SetOwner(Networking.LocalPlayer, driver.gameObject);
            if (actor) { Networking.SetOwner(Networking.LocalPlayer, actor.gameObject); }
            driver.IsDrive = true;
            saddle.EnabledAdjust = false;
            reins.InputEnabled = true;
        }

        internal void _OnExitSaddle()
        {
            driver.IsDrive = false;
            saddle.EnabledAdjust = false;
            reins.InputEnabled = false;
        }

        /******************************
         Bonds からのイベント
         ******************************/
        internal void _OnRemind(DragonBonds bonds)
        {

        }

        /******************************
         UI からのイベント
         ******************************/
        internal void _OnToggleChanged(RiderToggle toggler)
        {
            var toggleType = toggler.ToggleType;
            switch (toggleType)
            {
                case DragonRiderToggleType.SeatAdjust: break;
                case DragonRiderToggleType.InvertThrust: break;
                case DragonRiderToggleType.InvertClimb: break;
                case DragonRiderToggleType.InvertStrafe: break;
                case DragonRiderToggleType.InvertElevator: break;
                case DragonRiderToggleType.InvertLadder: break;
                case DragonRiderToggleType.InvertAileron: break;
                case DragonRiderToggleType.VRGrabMode: break;
                case DragonRiderToggleType.CanopyIndication: break;
            }
        }

        internal void _OnSelectChanged(RiderSelect selector)
        {
            var selectType = selector.SelectType;
            switch (selectType)
            {
                case DragonRiderSelectType.ReinsInput: break;
                case DragonRiderSelectType.ThrottleInputHand: break;
                case DragonRiderSelectType.TurningInputHand: break;
                case DragonRiderSelectType.ElevatorInputHand: break;
            }
        }

        internal void _OnVolumeChanged(RiderVolume volumer)
        {
            // 未使用
        }

        /******************************
         その他メソッド
         ******************************/
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

        internal void _Bond(DragonBonds bonds)
        {
            _bonds = bonds;
        }

        /******************************
         private メソッド
         ******************************/
        private void FeedbackAll()
        {

        }
    }
}
