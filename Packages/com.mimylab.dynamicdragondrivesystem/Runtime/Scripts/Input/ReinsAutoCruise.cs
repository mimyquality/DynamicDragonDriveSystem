/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Input/Reins AutoCruise")]
    public class ReinsAutoCruise : ReinsInputManager
    {
        public Transform targetPoint = null;
        [Min(0.0f)]
        public float targetSpeed = 60.0f;

        [SerializeField, Min(0.0f), Tooltip("meter")]
        private float _stoppingDistance = 10.0f;

        private Transform _dragonTF;

        private float _stillSpeedThreshold = 3.0f;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _dragonTF = _driver.transform;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        private protected override void InputKey()
        {
            if (!targetPoint) { return; }

            Vector3 direction = targetPoint.position - _dragonTF.position;
            // 加速指示
            _thrust = Accelerate(direction);
            // 方向指示
            _driver._InputDirectRotate(ConvertNoseDirection(direction), true);
        }

        private float Accelerate(Vector3 direction)
        {
            _brakes = false;

            float sqrCurrentSpeed = _driver.Velocity.sqrMagnitude;
            if (direction.sqrMagnitude < _stoppingDistance * _stoppingDistance)
            {
                if (sqrCurrentSpeed > _stillSpeedThreshold * _stillSpeedThreshold)
                {
                    _brakes = true;
                    return 0.0f;
                }
            }
            else
            {
                if (sqrCurrentSpeed < targetSpeed * targetSpeed)
                {
                    return 1.0f;
                }
            }

            return 0.0f;
        }

        private Vector3 ConvertNoseDirection(Vector3 direction)
        {
            Vector3 relativeDirection = Quaternion.Inverse(_dragonTF.rotation) * direction;
            Vector3 horizontalDirection = Vector3.ProjectOnPlane(relativeDirection, Vector3.up);
            float yaw = Vector3.SignedAngle(Vector3.forward, horizontalDirection, Vector3.up);
            float pitch = Vector3.SignedAngle(horizontalDirection, relativeDirection, Quaternion.LookRotation(horizontalDirection) * Vector3.right);

            return new Vector3(Mathf.Clamp(pitch, -90.0f, 90.0f), Mathf.Clamp(yaw, -90.0f, 90.0f));
        }
    }
}
