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

    public class DragonSaddle : DragonSeat
    {
        [HideInInspector]
        public DragonDriver driver;
        [HideInInspector]
        public DragonActor actor;
        [HideInInspector]
        public DragonReins reins;

        protected override void OnLocalPlayerMounted()
        {
            Networking.SetOwner(Networking.LocalPlayer, driver.gameObject);
            if (actor) { Networking.SetOwner(Networking.LocalPlayer, actor.gameObject); }
            driver.IsAwake = true;
            driver.enabled = true;
            reins.EnabledInput = true;
            EnabledAdjustInput = false;
        }

        protected override void OnLocalPlayerUnmounted()
        {
            driver.IsAwake = false;
            reins.EnabledInput = false;
        }

        protected override void OnMount()
        {
            if (actor) { actor.isMount = true; }
        }

        protected override void OnUnmount()
        {
            if (actor) { actor.isMount = false; }
        }

        protected override void OnEnableAdjust()
        {
            reins.EnabledInput = false;
        }

        protected override void OnDisableAdjust()
        {
            reins.EnabledInput = IsMount;
        }
    }
}
