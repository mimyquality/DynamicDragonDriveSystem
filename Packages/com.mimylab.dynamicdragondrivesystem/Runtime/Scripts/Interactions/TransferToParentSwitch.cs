/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Transfer to Parent Switch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TransferToParentSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform _parent;
        [SerializeField]
        private bool _returnWhenDisabled = false;

        private bool _isTransferredToParent = false;
        private Transform _defaultParent;

        private void Start()
        {
            if (_target) { _defaultParent = _target.parent; }
        }

        private void OnDisable()
        {
            if (_returnWhenDisabled)
            {
                ReturnToDefaultParent();
            }
        }

        public override void Interact()
        {
            SwitchParent(!_isTransferredToParent);
        }

        public void TransferToParent()
        {
            SwitchParent(true);
        }

        public void ReturnToDefaultParent()
        {
            SwitchParent(false);
        }

        private void SwitchParent(bool value)
        {
            if (!_target) { return; }
            if (!_parent) { return; }

            var destination = value ? _parent : _defaultParent;
            _target.SetParent(destination);
            _target.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            _isTransferredToParent = value;
        }
    }
}
