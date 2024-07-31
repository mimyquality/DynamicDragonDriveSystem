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
    //using VRC.SDK3.Components;
    using VRCStation = VRC.SDK3.Components.VRCStation;

    public enum SeatMountEventType
    {
        OnEntered,
        OnExited
    }

    public enum MountedPlayerType
    {
        All,
        Local,
        Owner,
        InstanceOwner,
        //Moderator
    }

    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Seat Mount EventTrigger")]
    [RequireComponent(typeof(VRCStation))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SeatMountEventTrigger : UdonSharpBehaviour
    {
        [SerializeField]
        SeatMountEventType _eventType;
        [SerializeField]
        private MountedPlayerType _mountedPlayerType;

        [SerializeField, Header("Activate/Inactivate GameObject")]
        private GameObject[] _activateObjects = new GameObject[0];
        [SerializeField]
        private bool _togetherSetOwner = false;
        [SerializeField]
        private GameObject[] _inactivateObjects = new GameObject[0];

        [Header("Execute SendCustomEvent to other UdonBehaviour")]
        [SerializeField,]
        private UdonBehaviour _udonBehaviour;
        [SerializeField]
        private string _eventName = "";

        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (_eventType == SeatMountEventType.OnEntered)
            {
                if (ValidateMountedPlayer(player)) { EventAction(_togetherSetOwner && player.isLocal); }
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            if (_eventType == SeatMountEventType.OnExited)
            {
                if (ValidateMountedPlayer(player)) { EventAction(_togetherSetOwner && player.isLocal); }
            }
        }

        private bool ValidateMountedPlayer(VRCPlayerApi player)
        {
            switch (_mountedPlayerType)
            {
                case MountedPlayerType.Local:
                    if (player.isLocal) { return true; }
                    break;
                case MountedPlayerType.Owner:
                    if (player.IsOwner(this.gameObject)) { return true; }
                    break;
                case MountedPlayerType.InstanceOwner:
                    if (player.isInstanceOwner) { return true; }
                    break;
                /* case MountedPlayerType.Moderator:
                    if (player.isModerator) { return true; }
                    break; */
                default:
                    return true;
            }

            return false;
        }

        private void EventAction(bool togetherSetOwner)
        {
            for (int i = 0; i < _activateObjects.Length; i++)
            {
                if (_activateObjects[i])
                {
                    _activateObjects[i].SetActive(true);
                    if (togetherSetOwner) { Networking.SetOwner(Networking.LocalPlayer, _activateObjects[i]); }
                }
            }
            for (int i = 0; i < _inactivateObjects.Length; i++)
            {
                if (_inactivateObjects[i])
                {
                    _inactivateObjects[i].SetActive(false);
                }
            }

            if (_udonBehaviour && _eventName != "") { _udonBehaviour.SendCustomEvent(_eventName); }
        }
    }
}
