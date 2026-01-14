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

            saddle._rider = rider;
            reins._driver = driver;
            rider._driver = driver;
            rider._saddle = saddle;
            rider._reins = reins;
            rider._actor = actor;
            if (actor)
            {
                actor._driver = driver;
                actor._reins = reins;
                actor._rider = rider;
            }
        }
    }
}
