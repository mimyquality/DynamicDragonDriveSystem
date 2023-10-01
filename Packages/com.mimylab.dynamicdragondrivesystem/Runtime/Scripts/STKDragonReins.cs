
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.Udon.Common;

namespace MimyLab.DynamicDragonDriveSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class STKDragonReins : DragonInputManager
    {
        [Range(0.0f, 1.0f), SerializeField]
        private float _brakesAcceptanceThreshold = 0.9f;
        private bool _InputBrakeLeft, _InputBrakeRight;

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            _thrust = value;
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            _InputBrakeLeft = value < -_brakesAcceptanceThreshold;
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            _elevator = -value;
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            _ladder = value;
            _aileron = -value;
            
            _InputBrakeRight = value > _brakesAcceptanceThreshold;
        }

        protected override void KeyInput()
        {
            _brakes = _InputBrakeLeft && _InputBrakeRight;
        }
    }
}
