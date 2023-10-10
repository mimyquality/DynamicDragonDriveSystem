/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace MimyLab.DynamicDragonDriveSystem
{
    public class DragonSaddle : DragonSeat
    {
        public DragonDriver driver;
        public DragonActor actor;
        public DragonReins reins;
        public SummonDragonSwitch summonSwitch;

        protected override void OnLocalPlayerMount()
        {
            Networking.SetOwner(Networking.LocalPlayer, driver.gameObject);
            Networking.SetOwner(Networking.LocalPlayer, actor.gameObject);
            driver.IsMount = true;
            driver.enabled = true;
            reins.IsEnabledInput = true;
            IsEnabledAdjustInput = false;
        }

        protected override void OnLocalPlayerUnmount()
        {
            driver.IsMount = false;
            reins.IsEnabledInput = false;
        }

        protected override void OnMount()
        {
            actor.isMount = true;
            summonSwitch.HasMounted = true;
        }

        protected override void OnUnmount()
        {
            actor.isMount = false;
            summonSwitch.HasMounted = false;
        }

        protected override void OnEnableAdjust()
        {
            reins.IsEnabledInput = false;
        }

        protected override void OnDisableAdjust()
        {
            reins.IsEnabledInput = IsMount;
        }
    }
}
