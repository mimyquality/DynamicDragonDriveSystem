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

    [DefaultExecutionOrder(-100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsInputManager : UdonSharpBehaviour
    {
        internal DragonDriver driver;

        protected float _thrust, _climb, _strafe;
        protected float _elevator, _ladder, _aileron;
        protected bool _brakes, _turbo;

        protected HandType _throttleInputHand = HandType.LEFT;
        protected HandType _turningInputHand = HandType.RIGHT;
        protected HandType _elevatorInputHand = HandType.RIGHT;

        protected Vector3 _accelerateSign = Vector3.one;
        protected Vector3 _rotateSign = Vector3.one;

        public virtual bool ThrustIsInvert { get => _accelerateSign.z < 0; }
        public virtual bool ClimbIsInvert { get => _accelerateSign.y < 0; }
        public virtual bool StrafeIsInvert { get => _accelerateSign.x < 0; }
        public virtual bool ElevatorIsInvert { get => _rotateSign.x < 0; }
        public virtual bool LadderIsInvert { get => _rotateSign.y < 0; }
        public virtual bool AileronIsInvert { get => _rotateSign.z < 0; }
        public virtual HandType ThrottleInputHand { get => _throttleInputHand; }
        public virtual HandType TurnInputHand { get => _turningInputHand; }
        public virtual HandType ElevatorInputHand { get => _elevatorInputHand; }
        public virtual float Thrust { get => _thrust; }
        public virtual float Turn { get => (driver.State == (int)DragonDriverStateType.Flight) ? -_aileron : _ladder; }
        public virtual float Elevator { get => -_elevator; }

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

        private void OnDisable()
        {
            if (!driver) { return; }

            Accelerate = Vector3.zero;
            Rotate = Vector3.zero;
            EmergencyBreakes = false;
            Overdrive = false;
            driver._InputDirectRotate(Vector3.zero);
        }

        private void Update()
        {
            if (!this.enabled) { return; }

            InputKey();

            Accelerate = new Vector3(_strafe, _climb, _thrust);
            Rotate = new Vector3(_elevator, _ladder, _aileron);
            EmergencyBreakes = _brakes;
            Overdrive = _turbo;
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (!value) { driver._InputJump(); }
        }

        public void _InvertThrust() { _accelerateSign.z = -1.0f; }
        public void _NormalThrust() { _accelerateSign.z = 1.0f; }

        public void _InvertClimb() { _accelerateSign.y = -1.0f; }
        public void _NormalClimb() { _accelerateSign.y = 1.0f; }

        public void _InvertStrafe() { _accelerateSign.x = -1.0f; }
        public void _NormalStrafe() { _accelerateSign.x = 1.0f; }

        public void _InvertElevator() { _rotateSign.x = -1.0f; }
        public void _NormalElevator() { _rotateSign.x = 1.0f; }

        public void _InvertLadder() { _rotateSign.y = -1.0f; }
        public void _NormalLadder() { _rotateSign.y = 1.0f; }

        public void _InvertAileron() { _rotateSign.z = -1.0f; }
        public void _NormalAileron() { _rotateSign.z = 1.0f; }

        public void _SetThrottleRightHand() { _throttleInputHand = HandType.RIGHT; }
        public void _SetThrottleLeftHand() { _throttleInputHand = HandType.LEFT; }

        public void _SetTurningRightHand() { _turningInputHand = HandType.RIGHT; }
        public void _SetTurningLeftHand() { _turningInputHand = HandType.LEFT; }

        public void _SetElevatorRightHand() { _elevatorInputHand = HandType.RIGHT; }
        public void _SetElevatorLeftHand() { _elevatorInputHand = HandType.LEFT; }

        protected virtual void InputKey() { }
    }
}
