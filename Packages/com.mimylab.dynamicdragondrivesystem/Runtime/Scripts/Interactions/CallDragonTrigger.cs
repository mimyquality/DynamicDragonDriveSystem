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

    [AddComponentMenu("Dynamic Dragon Drive System/Call Dragon Trigger")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CallDragonTrigger : UdonSharpBehaviour
    {
        [SerializeField]
        private Transform _target = null;
        private Transform Target { get => _target ? _target : this.transform; }

        private void OnTriggerEnter(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }

            Call(other.GetComponent<DragonDriver>());
        }

        public void Call(DragonDriver driver)
        {
            if (!driver) { return; }
            if (!Networking.IsOwner(driver.gameObject)) { return; }

            driver.TeleportTo(Target);
        }
    }
}
