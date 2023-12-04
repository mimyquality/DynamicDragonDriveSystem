/*
Copyright (c) 2023 Mimy Quality
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

    public enum MountedPlayerType
    {
        All,
        Local,
        Owner,
        InstanceOwner,
        Moderator
    }

    [RequireComponent(typeof(VRCStation))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Any)]
    public class SeatMountSender : UdonSharpBehaviour
    {
        [SerializeField]
        private MountedPlayerType _mountedPlayerType;

        [SerializeField]
        private GameObject[] _activateObjects = new GameObject[0];
        [SerializeField]
        private GameObject[] _inactivateObjects = new GameObject[0];

        private void Start()
        {
            ToggleActive(false);
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            switch (_mountedPlayerType)
            {
                case MountedPlayerType.Local: if (player.isLocal) { ToggleActive(true); } break;
                default: ToggleActive(true); break;
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            switch (_mountedPlayerType)
            {
                case MountedPlayerType.Local: if (player.isLocal) { ToggleActive(false); } break;
                default: ToggleActive(false); break;
            }
        }

        private void ToggleActive(bool activate)
        {
            for (int i = 0; i < _activateObjects.Length; i++)
            {
                if (_activateObjects[i]) { _activateObjects[i].SetActive(activate); }
            }
            for (int i = 0; i < _inactivateObjects.Length; i++)
            {
                if (_inactivateObjects[i]) { _inactivateObjects[i].SetActive(!activate); }
            }
        }
    }
}
