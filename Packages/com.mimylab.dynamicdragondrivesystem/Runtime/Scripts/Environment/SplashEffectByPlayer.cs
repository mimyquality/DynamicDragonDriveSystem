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
    [AddComponentMenu("Dynamic Dragon Drive System/Environment/SplashEffect by Player")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SplashEffectByPlayer : SplashEffect
    {
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) { return; }

            PlayEffect(player.GetPosition(), player.GetVelocity());
        }
    }
}
