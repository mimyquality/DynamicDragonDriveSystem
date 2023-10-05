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

namespace MimyLab.DynamicDragonDriveSystem
{
    [DefaultExecutionOrder(-1000)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DDDSDescriptor : UdonSharpBehaviour
    {
        public DragonDriver driver;
        public DragonSaddle saddle;
        public DragonActor actor;
        public DragonReins reins;
        public SeatAdjusterSwitch adjusterSwitch;
        public SummonDragonSwitch summonSwitch;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            driver = GetComponentInChildren<DragonDriver>(true);
            actor = GetComponentInChildren<DragonActor>(true);
            saddle = GetComponentInChildren<DragonSaddle>(true);
            reins = GetComponentInChildren<DragonReins>(true);
            adjusterSwitch = GetComponentInChildren<SeatAdjusterSwitch>(true);
            summonSwitch = GetComponentInChildren<SummonDragonSwitch>(true);

            actor.driver = driver;
            saddle.driver = driver;
            saddle.actor = actor;
            saddle.adjusterSwitch = adjusterSwitch;
            saddle.summonSwitch = summonSwitch;
            reins.driver = driver;
            reins.saddle = saddle;
            summonSwitch.driver = driver;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }
    }
}
