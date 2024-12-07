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
    //using VRC.Udon;
    using VRC.Udon.Common;

    [Icon(ComponentIconPath.DDDSystem)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SeatInputManager : UdonSharpBehaviour
    {
        private const float DoubleTapDuration = 0.2f;   // 単位：sec
        private const float TooltipShowTime = 5.0f; // 単位：sec

        internal bool disableInput;

        [SerializeField, Tooltip("sec"), Min(0.2f)]
        private float _exitAcceptance = 0.8f;
        [SerializeField]
        private GameObject _tooltipLock;
        [SerializeField]
        private GameObject _tooltipUnlock;

        private DragonSeat _seat;
        private bool _isVR = true;

        private bool _inputJump;
        private float _inputJumpTimer;
        private float _inputDoubleJumpTimer = DoubleTapDuration;

        private Vector3 _inputAdjust = Vector3.zero;

        private int _tooltipLockCount, _tooltipUnlockCount;

        private bool _lockAdjust = false;
        private bool LockAdjust
        {
            get => _lockAdjust;
            set
            {
                if (value)
                {
                    _inputAdjust = Vector3.zero;
                    ShowTooltipLock();
                }
                else
                {
                    ShowTooltipUnlock();
                }

                _lockAdjust = value;
            }
        }

        private void OnDisable()
        {
            _inputAdjust = Vector3.zero;
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
            if (!this.enabled) { return; }

            InputKey();

            _inputJumpTimer = _inputJump ? _inputJumpTimer + Time.deltaTime : 0.0f;
            if (_inputJumpTimer > _exitAcceptance) { SeatExit(); }
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
            if (disableInput) { return; }
            if (_lockAdjust) { return; }

            _inputAdjust.z = value;
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (disableInput) { return; }
            if (_lockAdjust) { return; }

            _inputAdjust.y = -value;
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            if (!_isVR) { return; }
            if (disableInput) { return; }
            if (_lockAdjust) { return; }

            _inputAdjust.x = value;
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            _inputJump = value;

            if (!disableInput && !value) { LockAdjust = !LockAdjust; }

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
            if (disableInput) { return; }
            if (_lockAdjust) { return; }

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
            if (_tooltipLockCount > 0) _tooltipLockCount--;
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
            if (_tooltipUnlockCount > 0) _tooltipUnlockCount--;
            if (_tooltipUnlockCount > 0) { return; }

            if (_tooltipUnlock) { _tooltipUnlock.SetActive(false); }
        }
    }
}
