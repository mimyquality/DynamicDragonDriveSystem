/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Transfer to Parent Switch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TransferToParentSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform _parent;

        [Header("Advanced Options")]
        [SerializeField]
        private bool _returnWhenDisabled = false;

        private Transform _defaultParent;
        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;
        private Vector3 _defaultScale;

        private void Start()
        {
            if (_target)
            {
                _defaultParent = _target.parent;
                _defaultPosition = _target.localPosition;
                _defaultRotation = _target.localRotation;
                _defaultScale = _target.localScale;
            }
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
            SwitchParent(true);
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

            if (value)
            {
                _target.SetParent(_parent);
                _target.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                return;
            }

            _target.SetParent(_defaultParent);
            _target.SetLocalPositionAndRotation(_defaultPosition, _defaultRotation);
            _target.localScale = _defaultScale;
        }
    }
}
