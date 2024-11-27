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
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Dragon Auto Returner")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonAutoReturner : UdonSharpBehaviour
    {
        [SerializeField]
        private DDDSDescriptor _target;

        [SerializeField, Min(0.0f)]
        private float _delayTime = 60.0f;
        [SerializeField]
        private bool _stayDragonIsGrounded = true;

        [Header("Option")]
        [SerializeField]
        private ReinsAutoCruise _motionController;
        [SerializeField, Min(0.0f)]
        private float _motionTime = 15.0f;

        private DragonDriver _driver;
        private DragonSaddle _saddle;
        private SphereCollider _collider;
        private float _currentDelayTime;
        private float _currentMotionTime;

        private bool _isReturned = true;
        private bool _isReturning = true;
        private bool IsReturning
        {
            get => _isReturning;
            set
            {
                if ((_isReturning != value) && _motionController)
                {
                    _motionController.enabled = value;
                    _motionController.gameObject.SetActive(value);
                    _collider.enabled = !value;
                    _driver.IsDrive = value;
                }
                _isReturning = value;
            }
        }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _driver = _target.driver;
            _saddle = _target.saddle;
            _collider = _driver.GetComponent<SphereCollider>();
            if (_motionController) { _motionController.driver = _driver; }

            _currentMotionTime = _delayTime + 10.0f;
            IsReturning = false;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (!Networking.IsOwner(_driver.gameObject))
            {
                IsReturning = false;
                return;
            }

            // 条件を満たしている間はリターン処理をリセット
            if (_saddle.IsMount || (_stayDragonIsGrounded && _driver.IsGrounded))
            {
                _currentDelayTime = 0.0f;
                _currentMotionTime = 0.0f;
                IsReturning = false;
                _isReturned = false;
                return;
            }

            // リターン処理済み判定
            if (_isReturned) { return; }

            // リターン判定のカウントスタート
            _currentDelayTime += Time.deltaTime;
            if (_currentDelayTime < _delayTime)
            {
                IsReturning = false;
                return;
            }

            // リターン移動中
            if (_motionController)
            {
                _currentMotionTime += Time.deltaTime;
                if (_currentMotionTime < _motionTime)
                {
                    IsReturning = true;
                    return;
                }
            }

            // リターン移動完了、リスポーンする
            IsReturning = false;
            _isReturned = true;
            _driver.Respawn();
        }
    }
}
