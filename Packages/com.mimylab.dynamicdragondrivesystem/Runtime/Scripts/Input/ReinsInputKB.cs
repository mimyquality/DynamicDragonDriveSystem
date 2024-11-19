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
    [AddComponentMenu("Dynamic Dragon Drive System/Input/ReinsInput Keyboard")]
    public class ReinsInputKB : ReinsInputManager
    {
        private void Reset()
        {
            _throttleInputHand = HandType.RIGHT;
            _turningInputHand = HandType.LEFT;
            _elevatorInputHand = HandType.LEFT;
        }

        // W・Sキー
        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (_throttleInputHand == HandType.LEFT) { _thrust = value; }
            if (_elevatorInputHand == HandType.LEFT) { _elevator = -value; }
        }

        // A・Dキー
        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (_turningInputHand == HandType.LEFT) { _ladder = value; }
            if (_turningInputHand == HandType.LEFT) { _aileron = -value; }
        }

        protected override void InputKey()
        {
            if (_throttleInputHand == HandType.RIGHT)
            {
                _thrust = Input.GetKey(KeyCode.E) ? 1.0f :
                          Input.GetKey(KeyCode.Q) ? -1.0f : 0.0f;
            }

            if (_elevatorInputHand == HandType.RIGHT)
            {
                _elevator = Input.GetKey(KeyCode.E) ? -1.0f :
                            Input.GetKey(KeyCode.Q) ? 1.0f : 0.0f;
            }

            _brakes = Input.GetKey(KeyCode.F);
        }
    }
}
