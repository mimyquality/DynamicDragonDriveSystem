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
    //using VRC.Udon;
    //using VRC.SDK3.Components;
    using VRCStation = VRC.SDK3.Components.VRCStation;

    [RequireComponent(typeof(VRCStation), typeof(SeatInputManager))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonSeat : UdonSharpBehaviour
    {
        [SerializeField]
        private bool _enabledAdjustInput = true;
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

                _adjustPoint.x = Mathf.Clamp(value.x, _minSeatAdjustment.x, _maxSeatAdjustment.x);
                _adjustPoint.y = Mathf.Clamp(value.y, _minSeatAdjustment.y, _maxSeatAdjustment.y);
                _adjustPoint.z = Mathf.Clamp(value.z, _minSeatAdjustment.z, _maxSeatAdjustment.z);

                _enterPoint.localPosition = _adjustPoint;

                RequestSerialization();
            }
        }

        public bool EnabledAdjustInput
        {
            get => _enabledAdjustInput;
            protected set
            {
                if (value && !_enabledAdjustInput) { OnEnableAdjust(); }
                if (!value && _enabledAdjustInput) { OnDisableAdjust(); }

                _enabledAdjustInput = value;
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

            EnabledAdjustInput = EnabledAdjustInput;
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
                AdjustPoint = _localAdjustPoint;
                _seatInput.enabled = true;
                this.DisableInteractive = true;

                OnLocalPlayerMounted();
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            IsMount = false;

            if (player.isLocal)
            {
                _localAdjustPoint = AdjustPoint;
                _seatInput.enabled = false;
                this.DisableInteractive = false;

                OnLocalPlayerUnmounted();
            }
        }

        public void _Ride()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
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

        public void _EnableSeatAdjust()
        {
            EnabledAdjustInput = true;
        }

        public void _DisableSeatAdjust()
        {
            EnabledAdjustInput = false;
        }

        protected virtual void OnLocalPlayerMounted() { }
        protected virtual void OnLocalPlayerUnmounted() { }
        protected virtual void OnMount() { }
        protected virtual void OnUnmount() { }
        protected virtual void OnEnableAdjust() { }
        protected virtual void OnDisableAdjust() { }

    }
}
