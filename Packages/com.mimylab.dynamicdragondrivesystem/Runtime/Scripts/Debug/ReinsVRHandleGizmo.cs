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

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Debug/ReinsInput VRHandle Gizmo")]
    [RequireComponent(typeof(Animator))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsVRHandleGizmo : UdonSharpBehaviour
    {
        [SerializeField]
        private ReinsInputVR2 _reinsVRHandle;

        [SerializeField]
        private Transform _inputGizmo;

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
            if (!_reinsVRHandle) { return; }

            _animator.SetFloat(_hashThrustName, _reinsVRHandle.Thrust);
            _animator.SetFloat(_hashTurnName, _reinsVRHandle.Turn);
            _animator.SetFloat(_hashElevatorName, _reinsVRHandle.Elevator);
        }

        public override void PostLateUpdate()
        {
            if (!_reinsVRHandle) { return; }

            Quaternion avatarRootRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).rotation;
            Vector3 leftHandPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
            Vector3 rightHandPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;

            Vector3 gizmoPosition = (leftHandPosition / 2) + (rightHandPosition / 2);
            _inputGizmo.SetPositionAndRotation(gizmoPosition, avatarRootRotation);

            if (_adjustToAvatarScale) { AdjustGizmoScale(); }
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        // AnimatorのApply Root Motion対策
        private void OnAnimatorMove() { }
#endif

        private void AdjustGizmoScale()
        {
            float avatarScale = Mathf.Max(_localPlayer.GetAvatarEyeHeightAsMeters(), 0.1f);
            if (avatarScale == _avatarScale) { return; }

            _inputGizmo.localScale = avatarScale * Vector3.one;

            _avatarScale = avatarScale;
        }
    }
}
