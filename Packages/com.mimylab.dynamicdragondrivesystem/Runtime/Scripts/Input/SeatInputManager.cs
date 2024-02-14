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
    using VRC.Udon.Common;
    //using VRC.SDK3.Components;

    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SeatInputManager : UdonSharpBehaviour
    {
        private const float DoubleTapDuration = 0.2f;   // 単位：sec
        private const float TooltipShowTime = 5.0f; // 単位：sec

        [Tooltip("sec"), Min(0.2f)]
        public float exitAcceptance = 0.8f;

        internal bool disabledAdjustLock = false;

        [SerializeField]
        private GameObject _tooltipLock;
        [SerializeField]
        private GameObject _tooltipUnlock;

        private DragonSeat _seat;
        private bool _isVR = true;

        private bool _inputJump;
        private float _inputJumpTimer;
        private float _inputDoubleJumpTimer = DoubleTapDuration;

        private Vector3 _inputAdjust;

        private int _tooltipLockCount, _tooltipUnlockCount;

        private bool _disabledAdjust;
        public bool DisabledAdjust
        {
            get => _disabledAdjust;
            set
            {
                if (value)
                {
                    _inputAdjust = Vector3.zero;
                }

                _disabledAdjust = value;
            }
        }

        private bool _lockedAdjust;
        private bool LockedAdjust
        {
            get => _lockedAdjust;
            set
            {
                if (disabledAdjustLock) { return; }

                if (value)
                {
                    _inputAdjust = Vector3.zero;
                    ShowTooltipLock();
                }
                else
                {
                    ShowTooltipUnlock();
                }

                _lockedAdjust = value;
            }
        }

        private void Start()
        {
            _seat = GetComponent<DragonSeat>();
            _isVR = Networking.LocalPlayer.IsUserInVR();

            if (_tooltipLock) { _tooltipLock.SetActive(false); }
            if (_tooltipUnlock) { _tooltipUnlock.SetActive(false); }

            // 未使用警告消す用
            if (_inputDoubleJumpTimer < 0.0f) { }
        }

        private void Update()
        {
            InputKey();

            _inputJumpTimer = _inputJump ? _inputJumpTimer + Time.deltaTime : 0.0f;
            if (_inputJumpTimer > exitAcceptance) { SeatExit(); }
#if UNITY_ANDROID
            // AndroidスマホはJumpボタン長押しが出来ないのでダブルタップ処理
            if (!_inputJump && _inputDoubleJumpTimer < DoubleTapDuration) { _inputDoubleJumpTimer += Time.deltaTime; }
#endif

            if (_inputAdjust != Vector3.zero)
            {
                _seat._SetAdjustPosition(_inputAdjust);
            }
        }

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (_disabledAdjust) { return; }
            if (_lockedAdjust) { return; }

            _inputAdjust.z = value;
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (_disabledAdjust) { return; }
            if (_lockedAdjust) { return; }

            _inputAdjust.y = -value;
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            if (!_isVR) { return; }
            if (_disabledAdjust) { return; }
            if (_lockedAdjust) { return; }

            _inputAdjust.x = value;
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            _inputJump = value;

            if (!_disabledAdjust && !value) { LockedAdjust = !LockedAdjust; }

#if UNITY_ANDROID
            // AndroidスマホはJumpボタン長押しが出来ないのでダブルタップ処理
            if (!_isVR && value)
            {
                if (_inputDoubleJumpTimer < DoubleTapDuration)
                {
                    SeatExit();
                    return;
                }
                _inputDoubleJumpTimer = 0.0f;
            }
#endif
        }

        private void InputKey()
        {
            if (_isVR) { return; }
            if (_disabledAdjust) { return; }
            if (_lockedAdjust) { return; }

            _inputAdjust.x = Input.GetKey(KeyCode.RightArrow) ? 1.0f :
                             Input.GetKey(KeyCode.LeftArrow) ? -1.0f : 0.0f;
        }

        private void SeatExit()
        {
            _seat._Exit();
            _inputJump = false;
            _inputDoubleJumpTimer = DoubleTapDuration;
            _inputAdjust = Vector3.zero;
        }

        private void ShowTooltipLock()
        {
            _tooltipLockCount++;
            if (_tooltipUnlock) { _tooltipUnlock.SetActive(false); }
            if (_tooltipLock) { _tooltipLock.SetActive(true); }
            SendCustomEventDelayedSeconds(nameof(_HideTooltipLockDelayed), TooltipShowTime);
        }
        public void _HideTooltipLockDelayed()
        {
            _tooltipLockCount = (_tooltipLockCount > 0) ? _tooltipLockCount - 1 : 0;
            if (_tooltipLockCount > 0) { return; }

            if (_tooltipLock) { _tooltipLock.SetActive(false); }
        }

        private void ShowTooltipUnlock()
        {
            _tooltipUnlockCount++;
            if (_tooltipLock) { _tooltipLock.SetActive(false); }
            if (_tooltipUnlock) { _tooltipUnlock.SetActive(true); }
            SendCustomEventDelayedSeconds(nameof(_HideTooltipUnlockDelayed), TooltipShowTime);
        }
        public void _HideTooltipUnlockDelayed()
        {
            _tooltipUnlockCount = (_tooltipUnlockCount > 0) ? _tooltipUnlockCount - 1 : 0;
            if (_tooltipUnlockCount > 0) { return; }

            if (_tooltipUnlock) { _tooltipUnlock.SetActive(false); }
        }
    }
}
