/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SnapToParentSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform _parent;

        public override void Interact()
        {
            if (_target)
            {
                SnapAndToggleActive(!_target.gameObject.activeSelf);
            }
        }

        public void SnapAndSetActive()
        {
            SnapAndToggleActive(true);
        }

        public void SnapAndInactive()
        {
            SnapAndToggleActive(false);
        }

        private void SnapAndToggleActive(bool v)
        {
            if (!_target) { return; }
            if (!_parent) { return; }

            _target.SetParent(_parent);
            _target.SetPositionAndRotation(_parent.position, _parent.rotation);
            _target.gameObject.SetActive(v);
        }
    }
}
