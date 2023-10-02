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
    public class DragonSaddle : UdonSharpBehaviour
    {
        public DragonDriver driver;
        public DragonActor actor;
        public DragonInputManager reins;
        public SummonDragonSwitch summonSwitch;

        [Tooltip("sec"), Min(0.2f)]
        public float exitAcceptance = 0.8f;

        private bool _isMount = false;
        public bool IsMount
        {
            get => _isMount;
            private set
            {
                summonSwitch.HasMounted = value;
                _isMount = value;
            }
        }

        private VRCPlayerApi _localPlayer;
        private VRCStation _station;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _localPlayer = Networking.LocalPlayer;
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
            _station.UseStation(_localPlayer);
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
                Networking.SetOwner(player, driver.gameObject);
                Networking.SetOwner(player, actor.gameObject);
                driver.IsMount = true;
                driver.enabled = true;
                reins.enabled = true;
            }

            IsMount = true;
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
                driver.IsMount = false;
                reins.enabled = false;
            }

            IsMount = false;
        }

        public void Exit()
        {
            if (IsMount)
            {
                _station.ExitStation(_localPlayer);
            }
        }
    }
}
