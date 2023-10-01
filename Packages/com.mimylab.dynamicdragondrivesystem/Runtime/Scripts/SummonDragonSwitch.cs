
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SummonDragonSwitch : UdonSharpBehaviour
    {
        public DragonDriver driver;

        [SerializeField]
        private float _interval = 5.0f;

        [FieldChangeCallback(nameof(HasMounted))]
        private bool _hasMounted;
        public bool HasMounted
        {
            get => _hasMounted;
            set
            {
                _hasMounted = value;
                this.DisableInteractive = _wasInteracted | value;
            }
        }

        private bool _wasInteracted = false;

        public override void Interact()
        {
            _wasInteracted = true;
            this.DisableInteractive = true;
            SendCustomEventDelayedSeconds(nameof(_ResetInteractInterval), _interval);
            driver.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DragonDriver.Summon));
        }

        public void _ResetInteractInterval()
        {
            _wasInteracted = false;
            this.DisableInteractive = _hasMounted;
        }
    }
}
