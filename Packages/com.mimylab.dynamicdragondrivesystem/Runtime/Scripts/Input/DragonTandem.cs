/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

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
        [Tooltip("sec"), Min(0.2f)]
        public float exitAcceptance = 0.8f;

        [Space]
        [SerializeField]
        private Vector3 _maxSeatAdjustment;
        [SerializeField]
        private Vector3 _minSeatAdjustment;

        private VRCStation _station;
        private Transform _enterPoint;
        private bool _isMount = false;
        private bool _isJumpContinuous = false;
        private bool _countingExitTimer = false;

        private float _adjustSpeed = 0.5f;
        private Vector3 _inputAdjust = Vector3.zero;
        private bool _isSleepAdjust = true;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _station = GetComponent<VRCStation>();
            _enterPoint = (_station.stationEnterPlayerLocation) ? _station.stationEnterPlayerLocation : _station.transform;

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
        }

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (!_isMount) { return; }

            _inputAdjust.z = value;

            if (_isSleepAdjust)
            {
                _isSleepAdjust = false;
                _AdjustUpdate();
            }
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (!_isMount) { return; }

            _inputAdjust.x = value;

            if (_isSleepAdjust)
            {
                _isSleepAdjust = false;
                _AdjustUpdate();
            }
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            if (!_isMount) { return; }

            _inputAdjust.y = value;

            if (_isSleepAdjust)
            {
                _isSleepAdjust = false;
                _AdjustUpdate();
            }
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (!_isMount) { return; }

            if (_countingExitTimer)
            {
                _isJumpContinuous = false;
            }
            else if (value)
            {
                _isJumpContinuous = true;
                _countingExitTimer = true;
                SendCustomEventDelayedSeconds(nameof(_ExitTimer), exitAcceptance);
            }
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (player.isLocal) { _isMount = true; }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            if (player.isLocal) { _isMount = false; }
        }

        public void _ExitTimer()
        {
            if (_isJumpContinuous)
            {
                _station.ExitStation(Networking.LocalPlayer);
            }

            _countingExitTimer = false;
        }

        public void _AdjustUpdate()
        {
            if (_inputAdjust == Vector3.zero)
            {
                _isSleepAdjust = true;
                return;
            }

            var adjustPosition = _enterPoint.localPosition;
            adjustPosition += Time.deltaTime * _adjustSpeed * _inputAdjust;

            adjustPosition.x = Mathf.Clamp(adjustPosition.x, _minSeatAdjustment.x, _maxSeatAdjustment.x);
            adjustPosition.y = Mathf.Clamp(adjustPosition.y, _minSeatAdjustment.y, _maxSeatAdjustment.y);
            adjustPosition.z = Mathf.Clamp(adjustPosition.z, _minSeatAdjustment.z, _maxSeatAdjustment.z);

            _enterPoint.localPosition = adjustPosition;

            SendCustomEventDelayedFrames(nameof(_AdjustUpdate), 1);
        }
    }
}
