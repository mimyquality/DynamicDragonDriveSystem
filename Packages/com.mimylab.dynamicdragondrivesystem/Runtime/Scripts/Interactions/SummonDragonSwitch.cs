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
    using VRC.Udon.Common.Interfaces;
    //using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/SummonDragon Switch")]
    [DefaultExecutionOrder(0)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SummonDragonSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        private DDDSDescriptor _target;
        [SerializeField]
        private float _interval = 5.0f;
        [SerializeField]
        private bool _UninteractiveOnAwake = true;

        private GameObject[] _children;
        private DragonDriver _driver;
        private DragonSaddle _saddle;
        private Collider _collider;
        private bool _isCoolDown;
        private bool _isStay;

        private void Start()
        {
            _children = new GameObject[transform.childCount];
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i] = transform.GetChild(i).gameObject;
            }

            _driver = _target.driver;
            _saddle = _target.saddle;
            _collider = _driver.GetComponent<Collider>();

            if (_UninteractiveOnAwake)
            {
                _isStay = true;
                ToggleInteractive();
            }
        }

        public override void Interact()
        {
            if (_saddle.IsMount) { return; }

            _isCoolDown = true;
            ToggleInteractive();
            SendCustomEventDelayedSeconds(nameof(_ResetInteractInterval), _interval);
            _driver.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DragonDriver.Respawn));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }
            if (other != _collider) { return; }

            _isStay = true;
            ToggleInteractive();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }
            if (other != _collider) { return; }

            _isStay = false;
            ToggleInteractive();
        }

        internal void _ResetInteractInterval()
        {
            _isCoolDown = false;
            ToggleInteractive();
        }

        private void ToggleInteractive()
        {
            var flag = _isCoolDown || _isStay;
            this.DisableInteractive = flag;
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].SetActive(!flag);
            }
        }
    }
}
