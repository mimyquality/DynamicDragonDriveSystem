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
    //using VRC.Udon;
    using VRC.Udon.Common;
    //using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Debug/ReinsInput VRLever Gizmo")]
    [RequireComponent(typeof(Animator))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsVRLeverGizmo : UdonSharpBehaviour
    {
        [SerializeField]
        private ReinsInputVR _reinsVRLever;

        [SerializeField]
        private Transform _throttleGizmo;
        [SerializeField]
        private Transform _turnGizmo;
        [SerializeField]
        private Transform _elevatorGizmo;

        [SerializeField]
        private bool _adjustToAvatarScale = true;

        private Animator _animator;
        private int _hashThrustName = Animator.StringToHash("Thrust");
        private int _hashTurnName = Animator.StringToHash("Turn");
        private int _hashElevatorName = Animator.StringToHash("Elevator");

        private VRCPlayerApi _localPlayer;
        private float _avatarScale;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _localPlayer = Networking.LocalPlayer;
        }

        private void Update()
        {
            if (!_reinsVRLever) { return; }

            _animator.SetFloat(_hashThrustName, _reinsVRLever.Thrust);
            _animator.SetFloat(_hashTurnName, _reinsVRLever.Turn);
            _animator.SetFloat(_hashElevatorName, _reinsVRLever.Elevator);
        }

        public override void PostLateUpdate()
        {
            if (!_reinsVRLever) { return; }

            var avatarRootRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).rotation;
            var leftHandPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
            var rightHandPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;

            var throttleGizmoPosition = (_reinsVRLever.ThrottleInputHand == HandType.LEFT) ? leftHandPosition : rightHandPosition;
            _throttleGizmo.SetPositionAndRotation(throttleGizmoPosition, avatarRootRotation);


            var turnGizmoPosition = (_reinsVRLever.TurnInputHand == HandType.LEFT) ? leftHandPosition : rightHandPosition;
            _turnGizmo.SetPositionAndRotation(turnGizmoPosition, avatarRootRotation);

            var elevatorGizmoPosition = (_reinsVRLever.ElevatorInputHand == HandType.LEFT) ? leftHandPosition : rightHandPosition;
            _elevatorGizmo.SetPositionAndRotation(elevatorGizmoPosition, avatarRootRotation);

            if (_adjustToAvatarScale) { AdjustGizmoScale(); }
        }

        // AnimatorのApply Root Motion対策
        private void OnAnimatorMove() { }

        private void AdjustGizmoScale()
        {
            var avatarScale = Mathf.Max(_localPlayer.GetAvatarEyeHeightAsMeters(), 0.1f);
            if (avatarScale == _avatarScale) { return; }

            _throttleGizmo.localScale = avatarScale * Vector3.one;
            _turnGizmo.localScale = avatarScale * Vector3.one;
            _elevatorGizmo.localScale = avatarScale * Vector3.one;

            _avatarScale = avatarScale;
        }
    }
}
