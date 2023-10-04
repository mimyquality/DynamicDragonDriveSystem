/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace MimyLab.DynamicDragonDriveSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
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
        private bool _isDT = true;

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
            _isDT = !Networking.LocalPlayer.IsUserInVR();
        }

        private void Update()
        {
            if (_isDT) { InputKey(); }

            _inputJumpTimer = (_inputJump) ? _inputJumpTimer + Time.deltaTime : 0.0f;

            if (_inputJumpTimer > exitAcceptance)
            {
                _seat._Exit();
                _inputJump = false;
                _inputAdjust = Vector3.zero;
            }

            _seat._AdjustPosition(_inputAdjust);
        }

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (_enableAdjustInput) { _inputAdjust.z = value; }
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (_enableAdjustInput) { _inputAdjust.x = value; }
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            if (_isDT) { return; }

            if (_enableAdjustInput) { _inputAdjust.y = value; }
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            _inputJump = value;
        }

        private void InputKey()
        {
            _inputAdjust.y = (Input.GetKey(KeyCode.UpArrow)) ? 1.0f :
                             (Input.GetKey(KeyCode.DownArrow)) ? -1.0f : 0.0f;
        }
    }
}
