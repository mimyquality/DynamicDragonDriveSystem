/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
//using VRC.SDKBase;
//using VRC.Udon;
//using VRC.SDK3.Components;
using VRC.Udon.Common.Interfaces;

namespace MimyLab.DynamicDragonDriveSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SummonDragonSwitch : UdonSharpBehaviour
    {
        public DragonSaddle saddle;

        [SerializeField]
        private float _interval = 5.0f;

        public override void Interact()
        {
            if (saddle.IsMount) { return; }

            this.DisableInteractive = true;
            SendCustomEventDelayedSeconds(nameof(_ResetInteractInterval), _interval);
            saddle.driver.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DragonDriver.Summon));
        }

        public void _ResetInteractInterval()
        {
            this.DisableInteractive = false;
        }
    }
}
