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
    using VRC.Udon;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Instruction/Rider Sub Category")]
    [DefaultExecutionOrder(-20)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RiderSubCategory : UdonSharpBehaviour
    {
        [SerializeField]
        internal DragonReinsInputType category;

        private void Start()
        {
            var toggles = GetComponentsInChildren<RiderInstructToggle>(true);
            foreach (var toggle in toggles)
            {
                toggle.targetReinsInput = category;
            }
            var selects = GetComponentsInChildren<RiderInstructSelect>(true);
            foreach (var select in selects)
            {
                select.targetReinsInput = category;
            }
            var volumes = GetComponentsInChildren<RiderInstructVolume>(true);
            foreach (var volume in volumes)
            {
                volume.targetReinsInput = category;
            }
        }
    }
}
