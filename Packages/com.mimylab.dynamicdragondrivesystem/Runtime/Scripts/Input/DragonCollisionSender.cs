/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
//using VRC.Udon;
//using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
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

            actor._TriggerCollision();
        }
    }
}
