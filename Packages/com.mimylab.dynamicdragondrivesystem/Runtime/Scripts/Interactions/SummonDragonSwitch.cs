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
    using VRC.Udon.Common.Interfaces;

    [AddComponentMenu("Dynamic Dragon Drive System/Summon Dragon Switch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SummonDragonSwitch : UdonSharpBehaviour
    {
        public DragonSaddle saddle;

        [SerializeField]
        private float _interval = 5.0f;
        [SerializeField]
        private bool _wakeOnUninteractive = true;

        private GameObject[] _children;
        private DragonDriver _driver;
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

            _driver = saddle.driver;
            _collider = _driver.GetComponent<Collider>();

            if (_wakeOnUninteractive)
            {
                _isStay = true;
                ToggleInteractive();
            }
        }

        public override void Interact()
        {
            if (saddle.IsMount) { return; }

            _isCoolDown = true;
            ToggleInteractive();
            SendCustomEventDelayedSeconds(nameof(_ResetInteractInterval), _interval);
            _driver.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DragonDriver.Summon));
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

        public void _ResetInteractInterval()
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
