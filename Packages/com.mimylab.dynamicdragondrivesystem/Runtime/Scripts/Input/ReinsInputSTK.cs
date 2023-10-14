/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
//using VRC.Udon;
using VRC.Udon.Common;
//using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsInputSTK : ReinsInputManager
    {
        [Range(0.0f, 1.0f), SerializeField]
        private float _brakesAcceptanceThreshold = 0.9f;
        private bool _InputBrakeLeft, _InputBrakeRight;

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (_throttleInputHand == HandType.LEFT) { _thrust = value; }
            if (_elevatorInputHand == HandType.LEFT) { _elevator = -value; }
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (_turningInputHand == HandType.LEFT) { _ladder = value; }
            if (_turningInputHand == HandType.LEFT) { _aileron = -value; }

            _InputBrakeLeft = value < -_brakesAcceptanceThreshold;
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            if (_throttleInputHand == HandType.RIGHT) { _thrust = value; }
            if (_elevatorInputHand == HandType.RIGHT) { _elevator = -value; }
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            if (_turningInputHand == HandType.RIGHT) { _ladder = value; }
            if (_turningInputHand == HandType.RIGHT) { _aileron = -value; }

            _InputBrakeRight = value > _brakesAcceptanceThreshold;
        }

        protected override void InputKey()
        {
            _brakes = _InputBrakeLeft && _InputBrakeRight;
        }
    }
}
