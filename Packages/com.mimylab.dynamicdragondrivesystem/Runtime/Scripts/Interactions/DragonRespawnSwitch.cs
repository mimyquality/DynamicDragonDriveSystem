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
    using VRC.Udon.Common.Interfaces;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Dragon Respawn Switch")]
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonRespawnSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        private DDDSDescriptor _target;
        [SerializeField]
        private float _interval = 5.0f;

        private GameObject[] _children;
        private DragonDriver _driver;
        private DragonSaddle _saddle;
        private Collider _dragonCollider;

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
            _dragonCollider = _driver.GetComponent<Collider>();

            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            SendCustomEventDelayedFrames(nameof(_EnableColliderDelayed), 2);
        }
        public void _EnableColliderDelayed()
        {
            _collider.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }
            if (other != _dragonCollider) { return; }

            _isStay = true;
            ToggleInteractive();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }
            if (other != _dragonCollider) { return; }

            _isStay = false;
            ToggleInteractive();
        }

        public override void Interact()
        {
            if (_saddle.IsMount) { return; }

            _isCoolDown = true;
            ToggleInteractive();
            SendCustomEventDelayedSeconds(nameof(_ResetInteractInterval), _interval);
            _driver.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DragonDriver.Respawn));
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
