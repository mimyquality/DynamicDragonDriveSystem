/*
Copyright (c) 2026 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRCStation = VRC.SDK3.Components.VRCStation;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Seat Suspension")]
    [RequireComponent(typeof(VRCStation))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SeatSuspension : UdonSharpBehaviour
    {
        // ダンパー調整パラメーター

        [Header("Advanced Options")]
        [SerializeField]
        private Transform _snapPoint;

        private bool _isInStation = false;
        
        private Transform _defaultParent;
        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _defaultParent = transform.parent;
            _defaultPosition = transform.localPosition;
            _defaultRotation = transform.localRotation;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        private void LateUpdate()
        {
            if (_isInStation)
            {
                // ダンパー処理

            }
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            Initialize();

            if (player.isLocal)
            {
                _isInStation = true;
            }
            else if (_snapPoint)
            {
                transform.SetParent(_snapPoint);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            Initialize();

            if (player.isLocal)
            {
                _isInStation = false;
            }
            else if (_snapPoint)
            {
                transform.SetParent(_defaultParent);
                transform.SetLocalPositionAndRotation(_defaultPosition, _defaultRotation);
            }
        }
    }
}
