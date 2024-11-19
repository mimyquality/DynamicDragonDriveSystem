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

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Input/ReinsInput Legacy")]
    public class ReinsInputLGC : ReinsInputManager
    {
        [SerializeField, Range(0.0f, 1.0f)]
        private float _brakesAcceptanceThreshold = 0.9f;
        private bool _InputBrakeLeft, _InputBrakeRight;

        private void Reset()
        {
            _throttleInputHand = HandType.RIGHT;
            _turningInputHand = HandType.LEFT;
            _elevatorInputHand = HandType.LEFT;
        }

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (_elevatorInputHand == HandType.LEFT) { _elevator = value; }
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (_throttleInputHand == HandType.LEFT) { _thrust = value; }
            if (_turningInputHand == HandType.LEFT) { _ladder = value; }
            if (_turningInputHand == HandType.LEFT) { _aileron = -value; }

            _InputBrakeLeft = value < -_brakesAcceptanceThreshold;
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            _InputBrakeRight = value < -_brakesAcceptanceThreshold;
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            if (_throttleInputHand == HandType.RIGHT) { _thrust = value; }
            if (_turningInputHand == HandType.RIGHT) { _ladder = value; }
            if (_turningInputHand == HandType.RIGHT) { _aileron = -value; }
        }

        protected override void InputKey()
        {
            _brakes = _InputBrakeLeft && _InputBrakeRight;
        }
    }
}
