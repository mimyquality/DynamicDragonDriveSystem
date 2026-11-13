/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

using System;
using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.SDK3.UdonNetworkCalling;
    using VRC.Udon.Common.Interfaces;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Dragon Collision Detector")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class DragonCollisionDetector : UdonSharpBehaviour
    {
        [SerializeField, Min(0.0f), Tooltip("m/s")]
        private float _collisionTriggerVelocity = 8.0f;
        [SerializeField, Range(0.0f, 180.0f), Tooltip("degree")]
        private float _collisionIncidenceAngle = 60.0f;

        private DragonDriver _driver;
        private DragonActor _actor;
        private Rigidbody _rigidbody;
        private VRCVelocityTracker _velocityTracker;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) return;

            var ddds = GetComponentInParent<DDDSDescriptor>(true);
            _driver = ddds.driver;
            _actor = ddds.actor;
            _rigidbody = _driver.GetComponent<Rigidbody>();
            _velocityTracker = _driver.GetComponent<VRCVelocityTracker>();

            _initialized = true;
        }

        private void Start()
        {
            Initialize();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!Utilities.IsValid(other)) return;
            if (!Utilities.IsValid(other.collider)) return;
            if (!Networking.IsOwner(_driver.gameObject)) return;

            if (_velocityTracker.GetLinearVelocity().sqrMagnitude < _collisionTriggerVelocity * _collisionTriggerVelocity) return;
            
            var noCollision = true;
            for (int i = 0; i < other.contactCount; i++)
            {
                if (Vector3.Angle(_rigidbody.rotation * Vector3.up, other.contacts[i].normal) > _collisionIncidenceAngle)
                {
                    noCollision = false;
                    break;
                }
            }
            if (noCollision) return;

            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TriggerCollision));
        }

        [NetworkCallable]
        public void TriggerCollision()
        {
            Initialize();

            if (_actor) { _actor._TriggerCollision(); }
        }
    }
}
