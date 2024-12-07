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

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Rider MenuSwitch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderMenuSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        private GameObject _menu;
        [SerializeField]
        private bool _isOpen = true;

        [Header("Animator Options")]
        [SerializeField]
        private string _parameter_IsOpen = "IsOpen";
        [SerializeField]
        private float _delayToClose = 1f;

        private Animator _animator;
        private int _parameterHash_IsOpen;

        private void Start()
        {
            _animator = _menu.GetComponent<Animator>();
            if (_animator)
            {
                _parameterHash_IsOpen = Animator.StringToHash(_parameter_IsOpen);
                _animator.SetBool(_parameterHash_IsOpen, _isOpen);

                if (!_isOpen)
                {
                    SendCustomEventDelayedSeconds(nameof(_CloseDelayed), _delayToClose);
                }

                return;
            }

            _menu.SetActive(_isOpen);
        }

        public override void Interact()
        {
            _isOpen = !_isOpen;

            if (_animator)
            {
                _animator.enabled = true;
                _animator.SetBool(_parameterHash_IsOpen, _isOpen);

                if (!_isOpen)
                {
                    SendCustomEventDelayedSeconds(nameof(_CloseDelayed), _delayToClose);
                }

                return;
            }

            _menu.SetActive(_isOpen);
        }

        public void _CloseDelayed()
        {
            if (!_isOpen)
            {
                _animator.enabled = false;
            }
        }
    }
}
