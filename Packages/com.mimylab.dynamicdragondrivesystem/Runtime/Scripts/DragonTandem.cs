
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRCStation = VRC.SDK3.Components.VRCStation;

namespace MimyLab.DynamicDragonDriveSystem
{
    [RequireComponent(typeof(VRC.SDK3.Components.VRCStation))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonTandem : UdonSharpBehaviour
    {
        private VRCStation _station;
        private bool _isMount = false;

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
            _station.UseStation(Networking.LocalPlayer);
            _isMount = true;
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (_isMount && value)
            {
                _station.ExitStation(Networking.LocalPlayer);
                _isMount = false;
            }
        }
    }
}
