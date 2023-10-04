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
    [RequireComponent(typeof(VRC.SDK3.Components.VRCStation))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonSeat : UdonSharpBehaviour
    {
        [Tooltip("sec"), Min(0.2f)]
        public float exitAcceptance = 0.8f;

        private bool _isMount = false;
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

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _station = GetComponent<VRCStation>();

            _station.PlayerMobility = VRCStation.Mobility.ImmobilizeForVehicle;
            _station.canUseStationFromStation = false;
            _station.disableStationExit = true;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        public override void Interact()
        {
            Ride();
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            IsMount = true;
            if (player.isLocal) { OnLocalPlayerStationEntered(); }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            IsMount = false;
            if (player.isLocal) { OnLocalPlayerStationExited(); }
        }

        public void Ride()
        {
            _station.UseStation(Networking.LocalPlayer);
        }

        public void Exit()
        {
            _station.ExitStation(Networking.LocalPlayer);
        }

        protected virtual void OnLocalPlayerStationEntered() { }
        protected virtual void OnLocalPlayerStationExited() { }
        protected virtual void OnMount() { }
        protected virtual void OnUnmount() { }
    }
}
