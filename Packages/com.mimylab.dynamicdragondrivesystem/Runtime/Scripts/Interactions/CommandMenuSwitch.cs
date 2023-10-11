
using UdonSharp;
using UnityEngine;

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
