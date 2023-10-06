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
        public SeatAdjusterSwitch adjusterSwitch;
        public SummonDragonSwitch summonSwitch;

        protected override void OnLocalPlayerMount()
        {
            Networking.SetOwner(Networking.LocalPlayer, driver.gameObject);
            Networking.SetOwner(Networking.LocalPlayer, actor.gameObject);
            driver.IsMount = true;
            driver.enabled = true;
            reins.EnableReinsInput = true;
            EnableSeatAdjust = false;
            adjusterSwitch.gameObject.SetActive(true);
        }

        protected override void OnLocalPlayerUnmount()
        {
            driver.IsMount = false;
            reins.EnableReinsInput = false;
            adjusterSwitch.gameObject.SetActive(false);
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
            reins.EnableReinsInput = false;
        }

        protected override void OnDisableAdjust()
        {
            reins.EnableReinsInput = IsMount;
        }
    }
}
