/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRCStation = VRC.SDK3.Components.VRCStation;

namespace MimyLab.DynamicDragonDriveSystem
{
    [RequireComponent(typeof(VRCStation), typeof(SeatInputManager))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonSeat : UdonSharpBehaviour
    {
        [SerializeField, FieldChangeCallback(nameof(EnableAdjustInput))]
        private bool _enableAdjustInput = true;
        [SerializeField]
        private Vector3 _maxSeatAdjustment = new Vector3(0.0f, 0.7f, 0.3f);
        [SerializeField]
        private Vector3 _minSeatAdjustment = new Vector3(0.0f, -0.3f, -0.3f);

        public bool EnableAdjustInput
        {
            get => _enableAdjustInput;
            set
            {
                if (value && !_enableAdjustInput) { OnEnableAdjust(); }
                if (!value && _enableAdjustInput) { OnDisableAdjust(); }

                _enableAdjustInput = value;
                _seatInput.EnableAdjustInput = value;
            }
        }

        public bool IsMount
        {
            get => _isMount;
            private set
            {
                if (value && !_isMount) { OnMount(); }
                if (!value && _isMount) { OnUnmount(); }

                _isMount = value;
            }
        }

        private VRCStation _station;
        private SeatInputManager _seatInput;
        private Transform _enterPoint;
        private bool _isMount = false;
        private float _adjustSpeed = 0.5f;  // m/s

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _station = GetComponent<VRCStation>();
            _seatInput = GetComponent<SeatInputManager>();
            _enterPoint = (_station.stationEnterPlayerLocation) ? _station.stationEnterPlayerLocation : _station.transform;

            _station.PlayerMobility = VRCStation.Mobility.ImmobilizeForVehicle;
            _station.canUseStationFromStation = false;
            _station.disableStationExit = true;

            EnableAdjustInput = EnableAdjustInput;
            _seatInput.enabled = false;

            _initialized = true;
        }
        protected virtual void Start()
        {
            Initialize();
        }

        public override void Interact()
        {
            _Ride();
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            IsMount = true;
            if (player.isLocal)
            {
                OnLocalPlayerStationEntered();
                _seatInput.enabled = true;
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            IsMount = false;
            if (player.isLocal)
            {
                OnLocalPlayerStationExited();
                _seatInput.enabled = false;
            }
        }

        public void _Ride()
        {
            _station.UseStation(Networking.LocalPlayer);
        }

        public void _Exit()
        {
            _station.ExitStation(Networking.LocalPlayer);
        }

        public void _AdjustPosition(Vector3 input)
        {
            if (input == Vector3.zero) { return; }

            var adjustPosition = _enterPoint.localPosition;
            adjustPosition += Time.deltaTime * _adjustSpeed * input;

            adjustPosition.x = Mathf.Clamp(adjustPosition.x, _minSeatAdjustment.x, _maxSeatAdjustment.x);
            adjustPosition.y = Mathf.Clamp(adjustPosition.y, _minSeatAdjustment.y, _maxSeatAdjustment.y);
            adjustPosition.z = Mathf.Clamp(adjustPosition.z, _minSeatAdjustment.z, _maxSeatAdjustment.z);

            _enterPoint.localPosition = adjustPosition;
        }

        protected virtual void OnLocalPlayerStationEntered() { }
        protected virtual void OnLocalPlayerStationExited() { }
        protected virtual void OnMount() { }
        protected virtual void OnUnmount() { }
        protected virtual void OnEnableAdjust() { }
        protected virtual void OnDisableAdjust() { }

    }
}
