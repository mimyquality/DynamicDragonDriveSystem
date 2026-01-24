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
        [SerializeField, Min(0.0f), Tooltip("meter")]
        private Vector3 _suspensionDistance = new Vector3(0.0f, 0.2f, 0.0f);
        [SerializeField, Min(0.0f)]
        private float _dampingRate = 0.1f;

        [Header("Advanced Options")]
        [SerializeField]
        private Transform _snapPoint;

        private bool _isInStation = false;

        private Transform _thisParent;
        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;

        private Vector3 _followPosition;
        private Quaternion _prevParentRotation;
        private Vector3 _currentVelocity;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _defaultPosition = transform.localPosition;
            _defaultRotation = transform.localRotation;

            _thisParent = transform.parent;
            _followPosition = transform.position;
            _prevParentRotation = _thisParent.rotation;

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
                SeatDamping();
            }
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            Initialize();

            if (player.isLocal)
            {
                _isInStation = true;
                _followPosition = transform.position;
                _prevParentRotation = _thisParent.rotation;

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
                transform.SetParent(_thisParent);
            }

            transform.SetLocalPositionAndRotation(_defaultPosition, _defaultRotation);
        }

        private void SeatDamping()
        {
            // ローカル座標化
            _followPosition = Quaternion.Inverse(_prevParentRotation) * _thisParent.rotation * _thisParent.InverseTransformPoint(_followPosition);

            _followPosition = Vector3.SmoothDamp(_followPosition, _defaultPosition, ref _currentVelocity, _dampingRate);
            _followPosition.x = Mathf.Clamp(_followPosition.x - _defaultPosition.x, -_suspensionDistance.x, _suspensionDistance.x) + _defaultPosition.x;
            _followPosition.y = Mathf.Clamp(_followPosition.y - _defaultPosition.y, -_suspensionDistance.y, _suspensionDistance.y) + _defaultPosition.y;
            _followPosition.z = Mathf.Clamp(_followPosition.z - _defaultPosition.z, -_suspensionDistance.z, _suspensionDistance.z) + _defaultPosition.z;
            transform.localPosition = _followPosition;

            _followPosition = _thisParent.TransformPoint(_followPosition);
            _prevParentRotation = _thisParent.rotation;
        }
    }
}
