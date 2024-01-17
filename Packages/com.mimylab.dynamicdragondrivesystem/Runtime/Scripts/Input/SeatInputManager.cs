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
        private const float DoubleTapDuration = 0.2f;    // 単位：sec

        [Tooltip("sec"), Min(0.2f)]
        public float exitAcceptance = 0.8f;

        public bool EnableAdjustInput
        {
            get => _enableAdjustInput;
            set => _enableAdjustInput = value;
        }

        private DragonSeat _seat;
        private bool _isVR = true;

        private bool _inputJump;
        private float _inputJumpTimer;
        private float _inputDoubleJumpTimer = DoubleTapDuration;

        private bool _enableAdjustInput;
        private Vector3 _inputAdjust;

        private void Start()
        {
            _seat = GetComponent<DragonSeat>();
            _isVR = Networking.LocalPlayer.IsUserInVR();

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
            if (!_enableAdjustInput) { return; }

            _inputAdjust.z = value;
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (!_enableAdjustInput) { return; }

            _inputAdjust.y = -value;
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            if (!_isVR) { return; }
            if (!_enableAdjustInput) { return; }

            _inputAdjust.x = value;
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            _inputJump = value;

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
            if (!_enableAdjustInput) { return; }

            _inputAdjust.x = Input.GetKey(KeyCode.RightArrow) ? 1.0f :
                             Input.GetKey(KeyCode.LeftArrow) ? -1.0f : 0.0f;
        }

        private void SeatExit()
        {
            _seat._Exit();
            _inputJump = false;
            _inputAdjust = Vector3.zero;
            _inputDoubleJumpTimer = DoubleTapDuration;
        }
    }
}
