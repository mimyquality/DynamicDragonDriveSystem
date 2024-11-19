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
    using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Bonds")]
    [RequireComponent(typeof(VRCPlayerObject))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonBonds : UdonSharpBehaviour
    {
        [SerializeField]
        private DDDSDescriptor _target;

        [Space]
        [SerializeField]
        private bool _enableLink = false;
        [SerializeField]
        private int _linkNumber = 0;

        private DragonRider _rider;

        public int LinkNumber { get => _linkNumber; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _rider = _target.rider;
            if (Networking.IsOwner(this.gameObject))
            {
                _rider._Remind(this);
            }

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }
    }
}
