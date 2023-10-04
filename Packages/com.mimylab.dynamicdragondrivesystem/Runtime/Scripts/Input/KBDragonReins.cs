/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.Udon.Common;

namespace MimyLab.DynamicDragonDriveSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class KBDragonReins : DragonInputManager
    {
        // W・Sキー
        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            _elevator = -value;
        }

        // A・Dキー
        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            _ladder = value;
            _aileron = -value;
        }

        protected override void InputKey()
        {
            _thrust = (Input.GetKey(KeyCode.E)) ? 1.0f :
                      (Input.GetKey(KeyCode.Q)) ? -1.0f : 0.0f;

            _brakes = Input.GetKey(KeyCode.F);
        }
    }
}
