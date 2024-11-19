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
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Misc/Reins Grab Helper")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsGrabHelper : UdonSharpBehaviour
    {
        [SerializeField]
        private DDDSDescriptor _target;

        [SerializeField]
        private Transform _leftGrabPoint;
        [SerializeField]
        private Transform _rightGrabPoint;
        [SerializeField]
        private bool _vrOnly = true;

        private DragonDriver _driver;
        private DragonSaddle _saddle;

        private Vector3 _defaultLeftReinsPosition, _defaultRightReinsPosition;
        private readonly float _returnSpeed = 1.0f;

        private void Start()
        {
            _driver = _target.driver;
            _saddle = _target.saddle;

            _defaultLeftReinsPosition = _leftGrabPoint.localPosition;
            _defaultRightReinsPosition = _rightGrabPoint.localPosition;
        }

        private void OnDisable()
        {
            _leftGrabPoint.localPosition = _defaultLeftReinsPosition;
            _rightGrabPoint.localPosition = _defaultRightReinsPosition;
        }

        public override void PostLateUpdate()
        {
            if (!gameObject.activeInHierarchy || !enabled) { return; }

            if (_saddle.IsMount)
            {
                var owner = Networking.GetOwner(_driver.gameObject);
                if (Utilities.IsValid(owner) && !(_vrOnly && !owner.IsUserInVR()))
                {
                    SnapLeftGrabHand(owner);
                    SnapRightGrabHand(owner);
                    return;
                }
            }

            ReturnLeftDefaultPoint();
            ReturnRightDefaultPoint();
        }

        private void SnapLeftGrabHand(VRCPlayerApi grabPlayer)
        {
            if (!_leftGrabPoint) { return; }

            var leftHandPosition = grabPlayer.GetBonePosition(HumanBodyBones.LeftHand);
            if (!leftHandPosition.Equals(Vector3.zero))
            {
                var leftFingerPosition = grabPlayer.GetBonePosition(HumanBodyBones.LeftMiddleIntermediate);
                if (leftFingerPosition.Equals(Vector3.zero))
                {
                    leftFingerPosition = grabPlayer.GetBonePosition(HumanBodyBones.LeftMiddleProximal);
                }
                if (!leftFingerPosition.Equals(Vector3.zero))
                {
                    leftHandPosition = (leftHandPosition + leftFingerPosition) / 2.0f;
                }
                _leftGrabPoint.position = leftHandPosition;
            }
        }

        private void SnapRightGrabHand(VRCPlayerApi grabPlayer)
        {
            if (!_rightGrabPoint) { return; }

            var rightHandPosition = grabPlayer.GetBonePosition(HumanBodyBones.RightHand);
            if (!rightHandPosition.Equals(Vector3.zero))
            {
                var rightFingerPosition = grabPlayer.GetBonePosition(HumanBodyBones.RightMiddleIntermediate);
                if (rightFingerPosition.Equals(Vector3.zero))
                {
                    rightFingerPosition = grabPlayer.GetBonePosition(HumanBodyBones.RightMiddleProximal);
                }
                if (!rightFingerPosition.Equals(Vector3.zero))
                {
                    rightHandPosition = (rightHandPosition + rightFingerPosition) / 2.0f;
                }
                _rightGrabPoint.position = rightHandPosition;
            }
        }

        private void ReturnLeftDefaultPoint()
        {
            if (!_leftGrabPoint) { return; }

            var currentPosition = _leftGrabPoint.localPosition;
            if (currentPosition == _defaultLeftReinsPosition) { return; }

            _leftGrabPoint.localPosition = Vector3.MoveTowards(currentPosition, _defaultLeftReinsPosition, _returnSpeed);
        }

        private void ReturnRightDefaultPoint()
        {
            if (!_rightGrabPoint) { return; }

            var currentPosition = _rightGrabPoint.localPosition;
            if (currentPosition == _defaultRightReinsPosition) { return; }

            _rightGrabPoint.localPosition = Vector3.MoveTowards(currentPosition, _defaultRightReinsPosition, _returnSpeed);
        }
    }
}
