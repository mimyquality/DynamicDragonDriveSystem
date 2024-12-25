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
    using VRC.Udon.Common;

    [Icon(ComponentIconPath.DDDSystem)]
    [DefaultExecutionOrder(-10)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsInputManager : UdonSharpBehaviour
    {
        internal DragonDriver driver;

        [SerializeField, HideInInspector]
        protected HandType _throttleInputHand = HandType.LEFT;
        [SerializeField, HideInInspector]
        protected HandType _turningInputHand = HandType.RIGHT;
        [SerializeField, HideInInspector]
        protected HandType _elevatorInputHand = HandType.RIGHT;

        protected float _thrust, _climb, _strafe;
        protected float _elevator, _ladder, _aileron;
        protected bool _brakes, _turbo;

        protected Vector3 _accelerateSign = Vector3.one;
        protected Vector3 _rotateSign = Vector3.one;

        public bool ThrustIsInvert
        {
            get => _accelerateSign.z < 0;
            set => _accelerateSign.z = value ? -1.0f : 1.0f;
        }
        public bool ClimbIsInvert
        {
            get => _accelerateSign.y < 0;
            set => _accelerateSign.y = value ? -1.0f : 1.0f;
        }
        public bool StrafeIsInvert
        {
            get => _accelerateSign.x < 0;
            set => _accelerateSign.x = value ? -1.0f : 1.0f;
        }
        public bool ElevatorIsInvert
        {
            get => _rotateSign.x < 0;
            set => _rotateSign.x = value ? -1.0f : 1.0f;
        }
        public bool LadderIsInvert
        {
            get => _rotateSign.y < 0;
            set => _rotateSign.y = value ? -1.0f : 1.0f;
        }
        public bool AileronIsInvert
        {
            get => _rotateSign.z < 0;
            set => _rotateSign.z = value ? -1.0f : 1.0f;
        }
        public HandType ThrottleInputHand
        {
            get => _throttleInputHand;
            set => _throttleInputHand = value;
        }
        public HandType TurnInputHand
        {
            get => _turningInputHand;
            set => _turningInputHand = value;
        }
        public HandType ElevatorInputHand
        {
            get => _elevatorInputHand;
            set => _elevatorInputHand = value;
        }

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

        protected virtual void InputKey() { }
    }
}
