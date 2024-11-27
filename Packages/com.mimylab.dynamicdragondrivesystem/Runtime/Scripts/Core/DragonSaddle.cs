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

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Saddle")]
    public class DragonSaddle : DragonSeat
    {
        protected override void Start()
        {
            base.Start();

            EnabledAdjust = false;
        }

        protected override void OnLocalPlayerMount()
        {
            rider._OnRideSaddle();
        }

        protected override void OnLocalPlayerUnmount()
        {
            rider._OnExitSaddle();
        }
    }
}
