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

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Misc/SplashEffect by Dragon")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SplashEffectByDragon : SplashEffect
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }

            var rb = other.attachedRigidbody;
            if (!rb) { return; }

            var driver = rb.GetComponent<DragonDriver>();
            if (!driver) { return; }

            PlayEffect(rb.position, driver.Velocity);
        }
    }
}
