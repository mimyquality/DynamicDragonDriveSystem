/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/CommandMenu Switch")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CommandMenuSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        private GameObject _commandMenu;

        public override void Interact()
        {
            _commandMenu.SetActive(!_commandMenu.activeSelf);
        }
    }
}
