/*
Copyright (c) 2023 Mimy Quality
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

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CommandMenuSwitch : UdonSharpBehaviour
    {
        [SerializeField]
        GameObject _commandMenu;

        public override void Interact()
        {
            _commandMenu.SetActive(!_commandMenu.activeSelf);
        }
    }
}
