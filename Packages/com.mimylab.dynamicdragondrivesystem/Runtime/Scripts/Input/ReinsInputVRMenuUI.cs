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

    [AddComponentMenu("Dynamic Dragon Drive System/Input/ReinsInputVR Menu UI")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsInputVRMenuUI : UdonSharpBehaviour
    {
        public ReinsInputVR reinsInputVR;
        public ReinsInputVR2 reinsInputVR2;
        [Space]
        [SerializeField] private GameObject _menu_grabHand_hold;
        [SerializeField] private GameObject _menu_grabHand_toggle;

        private void OnEnable()
        {
            if (reinsInputVR) { SetGrabMode(reinsInputVR._GetGrabMode()); }
            if (reinsInputVR2) { SetGrabMode(reinsInputVR2._GetGrabMode()); }
        }

        public void _SetGrabModeToggle()
        {
            if (reinsInputVR) { reinsInputVR._SetGrabModeToggle(); }
            if (reinsInputVR2) { reinsInputVR2._SetGrabModeToggle(); }
            SetGrabMode(true);
        }
        public void _SetGrabModeHold()
        {
            if (reinsInputVR) { reinsInputVR._SetGrabModeHold(); }
            if (reinsInputVR2) { reinsInputVR2._SetGrabModeHold(); }
            SetGrabMode(false);
        }
        private void SetGrabMode(bool isToggle)
        {
            if (_menu_grabHand_hold) { _menu_grabHand_hold.SetActive(!isToggle); }
            if (_menu_grabHand_toggle) { _menu_grabHand_toggle.SetActive(isToggle); }
        }
    }
}
