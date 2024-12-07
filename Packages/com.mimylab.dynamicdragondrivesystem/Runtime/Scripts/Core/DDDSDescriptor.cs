/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
    //using VRC.Udon;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/DDDS Descriptor")]
    [DefaultExecutionOrder(-100)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DDDSDescriptor : UdonSharpBehaviour
    {
        public DragonDriver driver;
        public DragonSaddle saddle;
        public DragonReins reins;
        public DragonRider rider;
        public DragonActor actor;

        private void Start()
        {
            if (!driver) { driver = GetComponentInChildren<DragonDriver>(true); }
            if (!saddle) { saddle = GetComponentInChildren<DragonSaddle>(true); }
            if (!reins) { reins = GetComponentInChildren<DragonReins>(true); }
            if (!rider) { rider = GetComponentInChildren<DragonRider>(true); }
            if (!actor) { actor = GetComponentInChildren<DragonActor>(true); }

            saddle.rider = rider;
            reins.driver = driver;
            rider.driver = driver;
            rider.saddle = saddle;
            rider.reins = reins;
            rider.actor = actor;
            if (actor)
            {
                actor.driver = driver;
                actor.reins = reins;
                actor.rider = rider;
            }
        }
    }
}
