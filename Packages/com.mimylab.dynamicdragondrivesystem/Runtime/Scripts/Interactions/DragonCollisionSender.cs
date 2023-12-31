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

    [UdonBehaviourSyncMode(BehaviourSyncMode.Any)]
    public class DragonCollisionSender : UdonSharpBehaviour
    {
        public DragonActor actor;

        private void Start()
        {
            if (!actor) actor = GetComponentInChildren<DragonActor>(true);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!Utilities.IsValid(other)) { return; }
            if (!Utilities.IsValid(other.collider)) { return; }
            if (!actor) { return; }

            actor._TriggerCollision(other);
        }
    }
}
