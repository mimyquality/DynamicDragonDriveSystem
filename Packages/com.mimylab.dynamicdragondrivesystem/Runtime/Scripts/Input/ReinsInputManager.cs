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

        private bool _inputJump;
        private float _inputJumpTimer;
        private float _exitAcceptance;

        private Vector3 _accelerate = Vector3.zero;
        private Vector3 Accelerate
        {
            get => _accelerate;
            set
            {
                if (_accelerate != value) { driver._InputAccelerate(value); }
                _accelerate = value;
            }
        }
        private Vector3 _rotate = Vector3.zero;
        private Vector3 Rotate
        {
            get => _rotate;
            set
            {
                if (_rotate != value) { driver._InputRotate(value); }
                _rotate = value;
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
            if (!value)
            {
                driver._InputJump();
            }
        }

        protected virtual void InputKey() { }
    }
}
