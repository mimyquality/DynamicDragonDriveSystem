/*
Copyright (c) 2023 Mimy Quality
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

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SeatInputManager : UdonSharpBehaviour
    {
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

        private bool _enableAdjustInput;
        private Vector3 _inputAdjust;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _initialized = true;
        }
        private void Start()
        {
            _seat = GetComponent<DragonSeat>();
            _isVR = Networking.LocalPlayer.IsUserInVR();
        }

        private void Update()
        {
            InputKey();

            _inputJumpTimer = (_inputJump) ? _inputJumpTimer + Time.deltaTime : 0.0f;

            if (_inputJumpTimer > exitAcceptance)
            {
                _seat._Exit();
                _inputJump = false;
                _inputAdjust = Vector3.zero;
            }

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
        }

        private void InputKey()
        {
            if (_isVR) { return; }
            if (!_enableAdjustInput) { return; }

            _inputAdjust.x = (Input.GetKey(KeyCode.RightArrow)) ? 1.0f :
                             (Input.GetKey(KeyCode.LeftArrow)) ? -1.0f : 0.0f;
        }
    }
}
