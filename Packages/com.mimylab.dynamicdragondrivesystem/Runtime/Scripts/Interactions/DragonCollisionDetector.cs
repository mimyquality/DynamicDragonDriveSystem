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
    //using VRC.SDK3.Components;

    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Dragon Collision Detector")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class DragonCollisionDetector : UdonSharpBehaviour
    {
        [SerializeField, Min(0.0f), Tooltip("m/s")]
        private float _collisionTriggerVelocity = 8.0f;
        [SerializeField, Range(0.0f, 180.0f), Tooltip("degree")]
        private float _collisionIncidenceAngle = 60.0f;

        private DragonDriver _driver;
        private DragonActor _actor;
        private Rigidbody _rigidbody;

        private void Start()
        {
            var descriptor = GetComponentInParent<DDDSDescriptor>();
            if (descriptor)
            {
                _driver = descriptor.driver;
                _actor = descriptor.actor;
                if (_driver) { _rigidbody = _driver.GetComponent<Rigidbody>(); }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!Utilities.IsValid(other)) { return; }
            if (!Utilities.IsValid(other.collider)) { return; }

            if (_driver.Velocity.sqrMagnitude < _collisionTriggerVelocity * _collisionTriggerVelocity) { return; }
            for (int i = 0; i < other.contactCount; i++)
            {
                if (Vector3.Angle(_rigidbody.rotation * Vector3.up, other.contacts[0].normal) < _collisionIncidenceAngle)
                {
                    return;
                }
            }

            if (_actor) { _actor._TriggerCollision(); }
        }
    }
}
