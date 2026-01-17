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
    using VRC.Udon.Common.Interfaces;
    using VRCStation = VRC.SDK3.Components.VRCStation;

    public enum SeatMountEventType
    {
        OnEnteredAndExited,
        OnEntered,
        OnExited
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
        private NetworkEventTarget _eventTarget;

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
                if (ValidateEventTarget(player))
                {
                    if (_withSetOwner && !_invert)
                    {
                        SetObjectsOwner(player);
                    }

                    ToggleActive(!_invert);
                    ExecuteOtherEvent();
                }
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            if (_eventType == SeatMountEventType.OnEnteredAndExited ||
                _eventType == SeatMountEventType.OnExited)
            {
                if (ValidateEventTarget(player))
                {
                    if (_withSetOwner && _invert)
                    {
                        SetObjectsOwner(player);
                    }

                    ToggleActive(_invert);
                    ExecuteOtherEvent();
                }
            }
        }

        private bool ValidateEventTarget(VRCPlayerApi player)
        {
            switch (_eventTarget)
            {
                case NetworkEventTarget.All:
                    return true;
                case NetworkEventTarget.Owner:
                    if (player.IsOwner(this.gameObject)) { return true; }
                    break;
                case NetworkEventTarget.Others:
                    if (!player.isLocal) { return true; }
                    break;
                case NetworkEventTarget.Self:
                    if (player.isLocal) { return true; }
                    break;
            }

            return false;
        }

        private void SetObjectsOwner(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }

            for (int i = 0; i < _gameObjects.Length; i++)
            {
                if (_gameObjects[i])
                {
                    Networking.SetOwner(player, _gameObjects[i]);
                }
            }
        }

        private void ToggleActive(bool value)
        {
            for (int i = 0; i < _gameObjects.Length; i++)
            {
                if (_gameObjects[i])
                {
                    _gameObjects[i].SetActive(value);
                }
            }
        }

        private void ExecuteOtherEvent()
        {
            if (_udonBehaviour && _eventName != "")
            {
                _udonBehaviour.SendCustomEvent(_eventName);
            }
        }
    }
}
