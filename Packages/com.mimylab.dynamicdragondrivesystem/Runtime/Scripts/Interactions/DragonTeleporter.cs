/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Dragon Teleporter")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonTeleporter : UdonSharpBehaviour
    {
        [SerializeField]
        private Transform _target = null;
        private Transform Target { get => _target ? _target : this.transform; }

        private void OnTriggerEnter(Collider other)
        {
            if (!Utilities.IsValid(other)) { return; }

            Teleport(other.GetComponent<DragonDriver>());
        }

        public void Teleport(DragonDriver driver)
        {
            if (!driver) { return; }
            if (!Networking.IsOwner(driver.gameObject)) { return; }

            driver.TeleportTo(Target);
        }
    }
}
