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
    using VRC.Udon;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider Sub Category")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderSubCategory : UdonSharpBehaviour
    {
        [SerializeField]
        internal DragonReinsInputType reinsInput;
    }
}
