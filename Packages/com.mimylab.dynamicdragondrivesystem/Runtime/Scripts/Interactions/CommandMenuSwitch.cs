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

namespace MimyLab.DynamicDragonDriveSystem
{
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
