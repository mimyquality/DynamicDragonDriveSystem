/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Environment/SplashEffect by Dragon")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SplashEffectByDragon : SplashEffect
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }

            var driver = other.GetComponent<DragonDriver>();
            if (!driver) { return; }

            PlayEffect(driver.transform.position, driver.Velocity);
        }
    }
}
