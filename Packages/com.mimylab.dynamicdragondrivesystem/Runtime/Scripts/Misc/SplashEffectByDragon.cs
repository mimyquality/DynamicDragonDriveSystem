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

    [AddComponentMenu("Dynamic Dragon Drive System/Splash Effect by Dragon")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SplashEffectByDragon : SplashEffect
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }

            var rb = other.attachedRigidbody;
            if (!rb) { return; }

            var dd = rb.GetComponent<DragonDriver>();
            if (!dd) { return; }

            PlayEffect(rb.position, dd.Velocity);
        }
    }
}
