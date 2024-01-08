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
    using VRC.Udon;
    //using VRC.SDK3.Components;

    [RequireComponent(typeof(SphereCollider))]
    [DefaultExecutionOrder(-100)]
    public class DragonCollisionDetector : UdonSharpBehaviour
    {
        internal DragonDriver driver;
        internal DragonActor actor;

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

        private Transform _transform;
        private SphereCollider _collider;

        private float _groundCheckRadius, _groundCheckRange;
        private RaycastHit _groundInfo = new RaycastHit();

        private void Start()
        {
            driver = GetComponent<DragonDriver>();
            actor = GetComponentInChildren<DragonActor>(true);
            _transform = transform;
            _collider = GetComponent<SphereCollider>();

            _groundCheckRadius = _collider.radius * 0.9f;
            _groundCheckRange = 2 * (_collider.radius - _groundCheckRadius);
        }

        private void FixedUpdate()
        {
            driver.IsGrounded = CheckGrounded();
            driver.groundInfo = _groundInfo;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!Utilities.IsValid(other)) { return; }
            if (!Utilities.IsValid(other.collider)) { return; }
            if (!actor) { return; }

            actor._TriggerCollision(other);
        }

        private bool CheckGrounded()
        {
            var origin = _transform.position + _transform.rotation * _collider.center;
            return Physics.SphereCast
                (
                    origin,
                    _groundCheckRadius,
                    Vector3.down,
                    out _groundInfo,
                    _groundCheckRange,
                    _groundLayer,
                    QueryTriggerInteraction.Ignore
                );
        }
    }
}
