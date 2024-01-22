﻿/*
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

    [AddComponentMenu("Dynamic Dragon Drive System/Dragon Collision Detector")]
    [RequireComponent(typeof(SphereCollider))]
    [DefaultExecutionOrder(-100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class DragonCollisionDetector : UdonSharpBehaviour
    {
        [SerializeField]
        private LayerMask _groundLayer =
            (1 << 0) |  // Default
            (1 << 1) |  // TransparentFX
            (1 << 2) |  // Ignore Raycast
            (1 << 4) |  // Water
            (1 << 8) |  // Interactive
            (1 << 11) | // Environment
            (1 << 15) | // StereoLeft
            (1 << 16) | // StereoRight
            (1 << 17);  // Walkthrough
        [SerializeField, Tooltip("degree"), Range(0.0f, 89.9f)]
        private float _slopeLimit = 45.0f;

        private DragonDriver _driver;
        private DragonActor _actor;
        private Transform _transform;
        private SphereCollider _collider;

        private Vector3 _colliderCenter;
        private float _groundCheckRadius, _groundCheckRange;
        private RaycastHit _groundInfo = new RaycastHit();
        private bool _wasGrounded;

        private void Start()
        {
            var descriptor = GetComponentInParent<DDDSDescriptor>();
            _driver = descriptor.driver;
            _actor = descriptor.actor;

            _transform = transform;
            _collider = GetComponent<SphereCollider>();

            _colliderCenter = _collider.center;
            _groundCheckRadius = _collider.radius * 0.9f;
            _groundCheckRange = 2 * (_collider.radius - _groundCheckRadius);
        }

        private void FixedUpdate()
        {
            if (!_driver) { return; }

            var isGrounded = CheckGrounded();
            if (isGrounded != _wasGrounded)
            {
                _driver.IsGrounded = isGrounded;
                if (Networking.IsOwner(_driver.gameObject)) { _driver.enabled = true; }

                _wasGrounded = isGrounded;
            }
            _driver.groundInfo = _groundInfo;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!Utilities.IsValid(other)) { return; }
            if (!Utilities.IsValid(other.collider)) { return; }
            if (!_actor) { return; }

            _actor._TriggerCollision(other);
        }

        private bool CheckGrounded()
        {
            var origin = _transform.position + _transform.rotation * _colliderCenter;
            if (Physics.SphereCast
                (
                    origin,
                    _groundCheckRadius,
                    Vector3.down,
                    out _groundInfo,
                    _groundCheckRange,
                    _groundLayer,
                    QueryTriggerInteraction.Ignore
                ))
            { return Vector3.Angle(Vector3.up, _groundInfo.normal) < _slopeLimit; }

            return false;
        }
    }
}
