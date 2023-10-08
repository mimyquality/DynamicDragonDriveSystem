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
    [DefaultExecutionOrder(-100)]
    public class ReinsInputManager : UdonSharpBehaviour
    {
        public DragonDriver driver;

        protected VRCPlayerApi _localPlayer;
        protected float _thrust, _lift, _lateral;
        protected float _elevator, _ladder, _aileron;
        protected bool _brakes, _turbo;

        protected HandType _throttleInputHand = HandType.LEFT;
        protected HandType _turningInputHand = HandType.RIGHT;
        protected HandType _elevatorInputHand = HandType.RIGHT;

        private bool _inputJump;
        private float _inputJumpTimer;
        private float _exitAcceptance;

        private Vector3 _accelerateSign = Vector3.one;
        private Vector3 _rotateSign = Vector3.one;

        private Vector3 _accelerate = Vector3.zero;
        private Vector3 Accelerate
        {
            get => _accelerate;
            set
            {
                if (_accelerate != value)
                {
                    driver._InputAccelerate(Vector3.Scale(_accelerateSign, value));
                    _accelerate = value;
                }
            }
        }
        private Vector3 _rotate = Vector3.zero;
        private Vector3 Rotate
        {
            get => _rotate;
            set
            {
                if (_rotate != value)
                {
                    driver._InputRotate(Vector3.Scale(_rotateSign, value));
                    _rotate = value;
                }
            }
        }
        private bool _emergencyBrakes = false;
        private bool EmergencyBreakes
        {
            get => _emergencyBrakes;
            set
            {
                if (_emergencyBrakes != value) { driver._InputEmergencyBrakes(value); }
                _emergencyBrakes = value;
            }
        }
        private bool _overdrive = false;
        private bool Overdrive
        {
            get => _overdrive;
            set
            {
                if (_overdrive != value) { driver._InputOverdrive(value); }
                _overdrive = value;
            }
        }

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
        }

        private void Update()
        {
            InputKey();

            Accelerate = new Vector3(_lateral, _lift, _thrust);
            Rotate = new Vector3(_elevator, _ladder, _aileron);
            EmergencyBreakes = _brakes;
            Overdrive = _turbo;
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (!value) { driver._InputJump(); }
        }

        public int GetThrustSign() { return (_accelerateSign.z > 0.0f) ? 1 : -1; }
        public void InvertThrust() { _accelerateSign.z = -1.0f; }
        public void NormalThrust() { _accelerateSign.z = 1.0f; }

        public int GetLiftSign() { return (_accelerateSign.y > 0.0f) ? 1 : -1; }
        public void InvertLift() { _accelerateSign.y = -1.0f; }
        public void NormalLift() { _accelerateSign.y = 1.0f; }

        public int GetLateralSign() { return (_accelerateSign.x > 0.0f) ? 1 : -1; }
        public void InvertLateral() { _accelerateSign.x = -1.0f; }
        public void NormalLateral() { _accelerateSign.x = 1.0f; }

        public int GetElevatorSign() { return (_rotateSign.x > 0.0f) ? 1 : -1; }
        public void InvertElevator() { _rotateSign.x = -1.0f; }
        public void NormalElevator() { _rotateSign.x = 1.0f; }

        public int GetLadderSign() { return (_rotateSign.y > 0.0f) ? 1 : -1; }
        public void InvertLadder() { _rotateSign.y = -1.0f; }
        public void NormalLadder() { _rotateSign.y = 1.0f; }

        public int GetAileronSign() { return (_rotateSign.z > 0.0f) ? 1 : -1; }
        public void InvertAileron() { _rotateSign.z = -1.0f; }
        public void NormalAileron() { _rotateSign.z = 1.0f; }

        public HandType GetThrottleHand() { return _throttleInputHand; }
        public void SetThrottleRightHand() { _throttleInputHand = HandType.RIGHT; }
        public void SetThrottleLeftHand() { _throttleInputHand = HandType.LEFT; }

        public HandType GetTurningHand() { return _turningInputHand; }
        public void SetTurningRightHand() { _turningInputHand = HandType.RIGHT; }
        public void SetTurningLeftHand() { _turningInputHand = HandType.LEFT; }

        public HandType GetElevatorHand() { return _elevatorInputHand; }
        public void SetElevatorRightHand() { _elevatorInputHand = HandType.RIGHT; }
        public void SetElevatorLeftHand() { _elevatorInputHand = HandType.LEFT; }

        protected virtual void InputKey() { }
    }
}
