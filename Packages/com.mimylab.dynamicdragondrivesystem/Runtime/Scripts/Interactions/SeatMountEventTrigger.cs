/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
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
        OnEnteredAndExited,
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

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Seat Mount EventTrigger")]
    [RequireComponent(typeof(VRCStation))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SeatMountEventTrigger : UdonSharpBehaviour
    {
        [SerializeField]
        SeatMountEventType _eventType;
        [SerializeField]
        private MountedPlayerType _mountedPlayerType;

        [SerializeField, Header("Activate GameObjects")]
        private GameObject[] _gameObjects = new GameObject[0];
        [SerializeField]
        private bool _invert = false;
        [SerializeField]
        private bool _withSetOwner = false;

        [Header("Execute SendCustomEvent to other UdonBehaviour")]
        [SerializeField,]
        private UdonBehaviour _udonBehaviour;
        [SerializeField]
        private string _eventName = "";

        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (_eventType == SeatMountEventType.OnEnteredAndExited ||
                _eventType == SeatMountEventType.OnEntered)
            {
                if (ValidateMountedPlayer(player))
                {
                    EventAction(!_invert);

                    if (_withSetOwner && !_invert && player.isLocal)
                    {
                        SetObjectsOwner(player);
                    }
                }
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            if (_eventType == SeatMountEventType.OnEnteredAndExited ||
                _eventType == SeatMountEventType.OnExited)
            {
                if (ValidateMountedPlayer(player))
                {
                    EventAction(_invert);

                    if (_withSetOwner && _invert && player.isLocal)
                    {
                        SetObjectsOwner(player);
                    }
                }
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

        private void EventAction(bool objectActive)
        {
            for (int i = 0; i < _gameObjects.Length; i++)
            {
                if (_gameObjects[i]) { _gameObjects[i].SetActive(objectActive); }
            }

            if (_udonBehaviour && _eventName != "") { _udonBehaviour.SendCustomEvent(_eventName); }
        }

        private void SetObjectsOwner(VRCPlayerApi player)
        {
            for (int i = 0; i < _gameObjects.Length; i++)
            {
                if (_gameObjects[i]) { Networking.SetOwner(player, _gameObjects[i]); }
            }
        }
    }
}
