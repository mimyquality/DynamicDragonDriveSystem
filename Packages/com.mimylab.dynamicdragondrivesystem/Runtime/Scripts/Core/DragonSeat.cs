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
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonSeat : UdonSharpBehaviour
    {
        [SerializeField, FieldChangeCallback(nameof(EnableSeatAdjust))]
        private bool _enableSeatAdjust = true;
        [SerializeField]
        private Vector3 _maxSeatAdjustment = new Vector3(0.0f, 0.7f, 0.3f);
        [SerializeField]
        private Vector3 _minSeatAdjustment = new Vector3(0.0f, -0.3f, -0.3f);

        [UdonSynced, FieldChangeCallback(nameof(AdjustPoint))]
        private Vector3 _adjustPoint;
        public Vector3 AdjustPoint
        {
            get => _adjustPoint;
            set
            {
                Initialize();

                _adjustPoint = value;
                _adjustPoint.x = Mathf.Clamp(_adjustPoint.x, _minSeatAdjustment.x, _maxSeatAdjustment.x);
                _adjustPoint.y = Mathf.Clamp(_adjustPoint.y, _minSeatAdjustment.y, _maxSeatAdjustment.y);
                _adjustPoint.z = Mathf.Clamp(_adjustPoint.z, _minSeatAdjustment.z, _maxSeatAdjustment.z);

                _enterPoint.localPosition = _adjustPoint;

                RequestSerialization();
            }
        }

        public bool EnableSeatAdjust
        {
            get => _enableSeatAdjust;
            set
            {
                if (value && !_enableSeatAdjust) { OnEnableAdjust(); }
                if (!value && _enableSeatAdjust) { OnDisableAdjust(); }

                _enableSeatAdjust = value;
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
        private Vector3 _localAdjustPoint;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _station = GetComponent<VRCStation>();
            _seatInput = GetComponent<SeatInputManager>();
            _enterPoint = (_station.stationEnterPlayerLocation) ? _station.stationEnterPlayerLocation : _station.transform;
            _localAdjustPoint = _enterPoint.localPosition;

            _station.PlayerMobility = VRCStation.Mobility.ImmobilizeForVehicle;
            _station.canUseStationFromStation = false;
            _station.disableStationExit = true;

            EnableSeatAdjust = EnableSeatAdjust;
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
                Networking.SetOwner(player, this.gameObject);
                AdjustPoint = _localAdjustPoint;
                _seatInput.enabled = true;

                OnLocalPlayerMount();
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            IsMount = false;

            if (player.isLocal)
            {
                _localAdjustPoint = AdjustPoint;
                _seatInput.enabled = false;

                OnLocalPlayerUnmount();
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

        public void _SetAdjustPosition(Vector3 input)
        {
            AdjustPoint += Time.deltaTime * _adjustSpeed * input;
        }

        protected virtual void OnLocalPlayerMount() { }
        protected virtual void OnLocalPlayerUnmount() { }
        protected virtual void OnMount() { }
        protected virtual void OnUnmount() { }
        protected virtual void OnEnableAdjust() { }
        protected virtual void OnDisableAdjust() { }

    }
}
