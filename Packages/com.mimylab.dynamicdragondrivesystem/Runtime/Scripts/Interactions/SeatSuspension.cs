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
        private Vector3 _suspensionDistance = new Vector3(0.0f, 0.1f, 0.0f);
        [SerializeField, Min(0.0f)]
        private float _dampingRate = 0.02f;

        [Header("Advanced Options")]
        [SerializeField]
        private Transform _snapPoint;

        private bool _emptyStation = true;
        private bool _isInStation = false;

        private Transform _thisParent;
        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;

        private Vector3 _followPosition;
        private Vector3 _currentVelocity;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _thisParent = transform.parent;
            _defaultPosition = transform.localPosition;
            _defaultRotation = transform.localRotation;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            if (_emptyStation) { enabled = false; }
        }

        private void LateUpdate()
        {
            if (_emptyStation) { return; }

            if (!_isInStation)
            {
                Snap();
            }
        }

        private void FixedUpdate()
        {
            if (_emptyStation) { return; }

            if (_isInStation)
            {
                Damp();
            }
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            Initialize();

            enabled = true;
            _emptyStation = false;

            if (player.isLocal)
            {
                _isInStation = true;
                _followPosition = transform.position;
            }
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            Initialize();

            enabled = false;
            _emptyStation = true;
            _isInStation = false;

            transform.SetLocalPositionAndRotation(_defaultPosition, _defaultRotation);
        }

        private void Damp()
        {
            // ローカル座標化
            Vector3 followPosition = transform.InverseTransformPoint(_followPosition);
            Vector3 targetPosition = transform.InverseTransformPoint(_thisParent.TransformPoint(_defaultPosition));

            Vector3 min = targetPosition - _suspensionDistance;
            Vector3 max = targetPosition + _suspensionDistance;
            followPosition.x = Mathf.Clamp(followPosition.x, min.x, max.x);
            followPosition.y = Mathf.Clamp(followPosition.y, min.y, max.y);
            followPosition.z = Mathf.Clamp(followPosition.z, min.z, max.z);

            followPosition = Vector3.SmoothDamp(followPosition, targetPosition, ref _currentVelocity, _dampingRate);

            // ワールド座標に戻して書き込み
            _followPosition = transform.TransformPoint(followPosition);
            transform.position = _followPosition;
        }

        private void Snap()
        {
            if (_snapPoint)
            {
                transform.SetPositionAndRotation(_snapPoint.position, _snapPoint.rotation);
            }
        }
    }
}
