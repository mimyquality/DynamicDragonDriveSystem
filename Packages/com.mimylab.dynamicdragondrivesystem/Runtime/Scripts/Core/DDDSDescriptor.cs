﻿/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
    //using VRC.Udon;

    [AddComponentMenu("Dynamic Dragon Drive System/Core/DDDS Descriptor")]
    [DefaultExecutionOrder(-1000)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DDDSDescriptor : UdonSharpBehaviour
    {
        public DragonDriver driver;
        public DragonSaddle saddle;
        public DragonActor actor;
        public DragonReins reins;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            if (!driver) driver = GetComponentInChildren<DragonDriver>(true);
            if (!actor) actor = GetComponentInChildren<DragonActor>(true);
            if (!saddle) saddle = GetComponentInChildren<DragonSaddle>(true);
            if (!reins) reins = GetComponentInChildren<DragonReins>(true);

            if (actor)
            {
                actor.driver = driver;
                actor.reins = reins;
            }
            saddle.driver = driver;
            saddle.actor = actor;
            saddle.reins = reins;
            reins.driver = driver;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }
    }
}
