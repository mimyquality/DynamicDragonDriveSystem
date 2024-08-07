﻿/*
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

    [AddComponentMenu("Dynamic Dragon Drive System/Misc/SplashEffect by Player")]
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
