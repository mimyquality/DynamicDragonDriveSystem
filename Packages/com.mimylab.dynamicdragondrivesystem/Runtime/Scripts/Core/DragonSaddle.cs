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

    [AddComponentMenu("Dynamic Dragon Drive System/Dragon Saddle")]
    public class DragonSaddle : DragonSeat
    {
        internal DragonDriver driver;
        internal DragonActor actor;
        internal DragonReins reins;

        protected override void Start()
        {
            base.Start();
            _seatInput.disabledAdjustLock = true;
            EnabledAdjust = false;
        }

        protected override void OnLocalPlayerMounted()
        {
            Networking.SetOwner(Networking.LocalPlayer, driver.gameObject);
            if (actor) { Networking.SetOwner(Networking.LocalPlayer, actor.gameObject); }
            driver.IsDrive = true;
            reins.InputEnabled = true;
        }

        protected override void OnLocalPlayerUnmounted()
        {
            driver.IsDrive = false;
            reins.InputEnabled = false;
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
            reins.InputEnabled = false;
        }

        protected override void OnDisableAdjust()
        {
            reins.InputEnabled = IsMount;
        }
    }
}
