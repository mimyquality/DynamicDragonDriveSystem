/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Saddle")]
    public class DragonSaddle : DragonSeat
    {
        private protected override void Start()
        {
            base.Start();

            EnableAdjust = false;
        }

        private protected override void OnLocalPlayerMount()
        {
            _rider._OnSaddleRided();
        }

        private protected override void OnLocalPlayerUnmount()
        {
            _rider._OnSaddleExited();
        }
    }
}
