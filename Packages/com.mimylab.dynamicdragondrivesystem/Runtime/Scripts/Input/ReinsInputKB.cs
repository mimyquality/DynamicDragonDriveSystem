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
    public class ReinsInputKB : ReinsInputManager
    {
        // W・Sキー
        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            if (_throttleInputHand == HandType.RIGHT) { _thrust = value; }
            if (_elevatorInputHand == HandType.RIGHT) { _elevator = -value; }
        }

        // A・Dキー
        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            if (_turningInputHand == HandType.RIGHT) { _ladder = value; }
            if (_turningInputHand == HandType.RIGHT) { _aileron = -value; }
        }

        protected override void InputKey()
        {
            if (_throttleInputHand == HandType.LEFT)
            {
                _thrust = (Input.GetKey(KeyCode.E)) ? 1.0f :
                          (Input.GetKey(KeyCode.Q)) ? -1.0f : 0.0f;
            }

            if (_elevatorInputHand == HandType.LEFT)
            {
                _elevator = (Input.GetKey(KeyCode.E)) ? 1.0f :
                            (Input.GetKey(KeyCode.Q)) ? -1.0f : 0.0f;
            }

            _brakes = Input.GetKey(KeyCode.F);
        }
    }
}
