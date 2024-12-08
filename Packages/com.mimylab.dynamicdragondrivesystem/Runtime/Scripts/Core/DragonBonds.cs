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
        private bool _enableSympathy = false;
        [SerializeField]
        private int _sympathyNumber = 0;

        // Saddle
        private Vector3 _seatPosition;

        // Reins
        private DragonReinsInputType _selectedInput;
        private byte[] _throttleInputHand = new byte[6];
        private byte[] _turningInputHand = new byte[6];
        private byte[] _elevatorInputHand = new byte[6];
        private bool[] _invertThrust = new bool[6];
        private bool[] _invertClimb = new bool[6];
        private bool[] _invertStrafe = new bool[6];
        private bool[] _invertElevator = new bool[6];
        private bool[] _invertLadder = new bool[6];
        private bool[] _invertAileron = new bool[6];
        private bool[] _vrGrabMode = new bool[2];

        // Canopy
        private bool _canopyIndication;

        private DragonRider _rider;

        internal int SympathyNumber { get => _sympathyNumber; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _rider = _target.rider;
            if (Networking.IsOwner(this.gameObject))
            {
                _rider._Bond(this);
            }

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        internal void _Memorize() { }
    }
}
