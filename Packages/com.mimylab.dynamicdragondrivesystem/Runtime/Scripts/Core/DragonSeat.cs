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

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Seat")]
    [RequireComponent(typeof(VRCStation), typeof(SeatInputManager))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonSeat : UdonSharpBehaviour
    {
        internal DragonRider rider;

        [SerializeField]
        private bool _enabledAdjust = true;
        [SerializeField]
        private Vector3 _maxSeatAdjustment = new Vector3(0.0f, 0.7f, 0.3f);
        [SerializeField]
        private Vector3 _minSeatAdjustment = new Vector3(0.0f, -0.3f, -0.3f);

        [Space]
        [SerializeField]
        private Transform _snapPoint;

        [UdonSynced, FieldChangeCallback(nameof(AdjustPoint))]
        private Vector3 _adjustPoint;

        protected VRCStation _station;
        protected SeatInputManager _seatInput;
        private Transform _enterPoint;
        private bool _isRide = false;
        private bool _isMount = false;
        private int _mountedPlayerId = -1;
        private float _adjustSpeed = 0.5f;  // m/s
        private Vector3 _localAdjustPoint;
        private Transform _defaultParent;
        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;

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

        public bool EnabledAdjust
        {
            get => _enabledAdjust;
            set
            {
                if (_enabledAdjust != value)
                {
                    _seatInput.disableInput = !value;
                    _enabledAdjust = value;
                }
            }
        }

        public bool IsRide { get => _isRide; }
        public bool IsMount { get => _isMount; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            if (!rider) { rider = GetComponentInParent<DDDSDescriptor>(true).rider; }

            _station = GetComponent<VRCStation>();
            _seatInput = GetComponent<SeatInputManager>();
            _enterPoint = _station.stationEnterPlayerLocation ? _station.stationEnterPlayerLocation : _station.transform;
            _localAdjustPoint = _enterPoint.localPosition;

            _defaultParent = transform.parent;
            _defaultPosition = transform.localPosition;
            _defaultRotation = transform.localRotation;

            _station.PlayerMobility = VRCStation.Mobility.ImmobilizeForVehicle;
            _station.canUseStationFromStation = false;
            _station.disableStationExit = true;

            _seatInput.enabled = false;
            _seatInput.disableInput = !_enabledAdjust;

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

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            Initialize();

            if (player.playerId == _mountedPlayerId)
            {
                OnPlayerUnmount(player);
            }
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            Initialize();

            OnPlayerMount(player);
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            Initialize();

            OnPlayerUnmount(player);
        }

        public void _Ride()
        {
            Initialize();

            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            _station.UseStation(Networking.LocalPlayer);
        }

        public void _Exit()
        {
            Initialize();

            _station.ExitStation(Networking.LocalPlayer);
        }

        public void _SetAdjustPosition(Vector3 input)
        {
            Initialize();

            AdjustPoint += Time.deltaTime * _adjustSpeed * input;
        }

        protected virtual void OnPlayerMount(VRCPlayerApi player)
        {
            _isMount = true;
            _mountedPlayerId = player.playerId;
            rider._SetIsMount(true);

            if (player.isLocal)
            {
                _isRide = true;
                AdjustPoint = _localAdjustPoint;
                _seatInput.enabled = true;
                this.DisableInteractive = true;
                rider._SetIsRide(true);

                OnLocalPlayerMount();
            }
            else if (_snapPoint)
            {
                transform.SetParent(_snapPoint);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        protected virtual void OnPlayerUnmount(VRCPlayerApi player)
        {
            _isMount = false;
            _mountedPlayerId = -1;
            rider._SetIsMount(false);

            if (player.isLocal)
            {
                _isRide = false;
                _localAdjustPoint = AdjustPoint;
                _seatInput.enabled = false;
                this.DisableInteractive = false;
                rider._SetIsRide(false);

                OnLocalPlayerUnmount();
            }
            else if (_snapPoint)
            {
                transform.SetParent(_defaultParent);
                transform.SetLocalPositionAndRotation(_defaultPosition, _defaultRotation);
            }
        }

        protected virtual void OnLocalPlayerMount() { }
        protected virtual void OnLocalPlayerUnmount() { }
    }
}
