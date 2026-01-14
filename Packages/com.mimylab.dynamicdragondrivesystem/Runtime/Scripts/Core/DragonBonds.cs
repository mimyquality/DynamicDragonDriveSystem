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
    using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Bonds")]
    [RequireComponent(typeof(VRCPlayerObject), typeof(VRCEnablePersistence))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonBonds : UdonSharpBehaviour
    {
        private const float RememberWaitTime = 2.0f;

        [SerializeField]
        internal bool _enableSympathize = false;
        [SerializeField]
        internal int _sympathyCircuit = 0;

        [UdonSynced] private Vector3 sync_seatPosition;
        [UdonSynced]
        private int[] sync_selectedInput = new int[]
        {
            (int)DragonReinsInputType.None,
            (int)DragonReinsInputType.None
        };
        [UdonSynced] private byte[] sync_throttleInputHand = new byte[(int)DragonReinsInputType.Count];
        [UdonSynced] private byte[] sync_turningInputHand = new byte[(int)DragonReinsInputType.Count];
        [UdonSynced] private byte[] sync_elevatorInputHand = new byte[(int)DragonReinsInputType.Count];
        [UdonSynced] private bool[] sync_invertThrust = new bool[(int)DragonReinsInputType.Count];
        [UdonSynced] private bool[] sync_invertClimb = new bool[(int)DragonReinsInputType.Count];
        [UdonSynced] private bool[] sync_invertStrafe = new bool[(int)DragonReinsInputType.Count];
        [UdonSynced] private bool[] sync_invertElevator = new bool[(int)DragonReinsInputType.Count];
        [UdonSynced] private bool[] sync_invertLadder = new bool[(int)DragonReinsInputType.Count];
        [UdonSynced] private bool[] sync_invertAileron = new bool[(int)DragonReinsInputType.Count];
        [UdonSynced] private byte[] sync_vrGrabMode = new byte[2];
        [UdonSynced] private bool sync_canopyIndication;

        // Saddle
        internal Vector3 _seatPosition;
        // Reins
        internal DragonReinsInputType[] _selectedInput = new DragonReinsInputType[]
        {
            DragonReinsInputType.None,
            DragonReinsInputType.None
        };
        // HandType[]が扱いにくいのでint[]でやりとりする
        internal int[] _throttleInputHand = new int[(int)DragonReinsInputType.Count];
        internal int[] _turningInputHand = new int[(int)DragonReinsInputType.Count];
        internal int[] _elevatorInputHand = new int[(int)DragonReinsInputType.Count];
        internal bool[] _invertThrust = new bool[(int)DragonReinsInputType.Count];
        internal bool[] _invertClimb = new bool[(int)DragonReinsInputType.Count];
        internal bool[] _invertStrafe = new bool[(int)DragonReinsInputType.Count];
        internal bool[] _invertElevator = new bool[(int)DragonReinsInputType.Count];
        internal bool[] _invertLadder = new bool[(int)DragonReinsInputType.Count];
        internal bool[] _invertAileron = new bool[(int)DragonReinsInputType.Count];
        internal ReinsInputVRGrabMode[] _vrGrabMode = new ReinsInputVRGrabMode[2];
        // Canopy
        internal bool _canopyIndication;

        private DragonRider _rider;
        private DragonBonds[] _sympathizedBonds;
        private bool _waitingToRemember = false;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            if (Networking.IsOwner(this.gameObject))
            {
                _rider = GetComponentInParent<DDDSDescriptor>(true).rider;
                _rider._Bond(this);
            }

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }

        public override void OnPreSerialization()
        {
            sync_seatPosition = _seatPosition;

            var select = (int)_selectedInput[0];
            sync_selectedInput[0] = select;
            select = (int)_selectedInput[1];
            sync_selectedInput[1] = select;

            for (int i = 0; i < (int)DragonReinsInputType.Count; i++)
            {
                select = _throttleInputHand[i];
                sync_throttleInputHand[i] = (byte)select;
                select = _turningInputHand[i];
                sync_turningInputHand[i] = (byte)select;
                select = _elevatorInputHand[i];
                sync_elevatorInputHand[i] = (byte)select;

                sync_invertThrust[i] = _invertThrust[i];
                sync_invertClimb[i] = _invertClimb[i];
                sync_invertStrafe[i] = _invertStrafe[i];
                sync_invertElevator[i] = _invertElevator[i];
                sync_invertLadder[i] = _invertLadder[i];
                sync_invertAileron[i] = _invertAileron[i];
            }

            select = (int)_vrGrabMode[0];
            sync_vrGrabMode[0] = (byte)select;
            select = (int)_vrGrabMode[1];
            sync_vrGrabMode[1] = (byte)select;

            sync_canopyIndication = _canopyIndication;
        }

        public override void OnDeserialization()
        {
            Initialize();

            _seatPosition = sync_seatPosition;

            int tmp_selectedInput = sync_selectedInput[0];
            _selectedInput[0] = (DragonReinsInputType)tmp_selectedInput;
            tmp_selectedInput = sync_selectedInput[1];
            _selectedInput[1] = (DragonReinsInputType)tmp_selectedInput;

            for (int i = 0; i < (int)DragonReinsInputType.Count; i++)
            {
                _throttleInputHand[i] = (int)sync_throttleInputHand[i];
                _turningInputHand[i] = (int)sync_turningInputHand[i];
                _elevatorInputHand[i] = (int)sync_elevatorInputHand[i];

                _invertThrust[i] = sync_invertThrust[i];
                _invertClimb[i] = sync_invertClimb[i];
                _invertStrafe[i] = sync_invertStrafe[i];
                _invertElevator[i] = sync_invertElevator[i];
                _invertLadder[i] = sync_invertLadder[i];
                _invertAileron[i] = sync_invertAileron[i];
            }

            _vrGrabMode[0] = (ReinsInputVRGrabMode)sync_vrGrabMode[0];
            _vrGrabMode[1] = (ReinsInputVRGrabMode)sync_vrGrabMode[1];

            _canopyIndication = sync_canopyIndication;

            if (Networking.IsOwner(this.gameObject))
            {
                _rider._OnRemind(this);
            }
        }

        // Riderの方から一通り変更値を書き込み後、最後に実行(RequestSerialization()感覚で使う)
        internal void _Remember()
        {
            if (_waitingToRemember) { return; }

            _waitingToRemember = true;
            SendCustomEventDelayedSeconds(nameof(_KeepInMind), RememberWaitTime);
        }
        public void _KeepInMind()
        {
            _waitingToRemember = false;
            RequestSerialization();

            if (_enableSympathize)
            {
                if (_sympathizedBonds == null) { _sympathizedBonds = GetSympathizedBonds(); }

                foreach (DragonBonds bonds in _sympathizedBonds)
                {
                    bonds._Sympathize(this);
                }
            }
        }

        internal void _Sympathize(DragonBonds memorizedBonds)
        {
            Initialize();

            // Saddle
            _seatPosition = memorizedBonds._seatPosition;
            // Reins
            memorizedBonds._selectedInput.CopyTo(_selectedInput, 0);
            memorizedBonds._throttleInputHand.CopyTo(_throttleInputHand, 0);
            memorizedBonds._turningInputHand.CopyTo(_turningInputHand, 0);
            memorizedBonds._elevatorInputHand.CopyTo(_elevatorInputHand, 0);
            memorizedBonds._invertThrust.CopyTo(_invertThrust, 0);
            memorizedBonds._invertClimb.CopyTo(_invertClimb, 0);
            memorizedBonds._invertStrafe.CopyTo(_invertStrafe, 0);
            memorizedBonds._invertElevator.CopyTo(_invertElevator, 0);
            memorizedBonds._invertLadder.CopyTo(_invertLadder, 0);
            memorizedBonds._invertAileron.CopyTo(_invertAileron, 0);
            memorizedBonds._vrGrabMode.CopyTo(_vrGrabMode, 0);
            // Canopy
            _canopyIndication = memorizedBonds._canopyIndication;

            RequestSerialization();

            _rider._OnRemind(this);
        }

        private DragonBonds[] GetSympathizedBonds()
        {
            GameObject[] playerObjects = Networking.LocalPlayer.GetPlayerObjects();
            var sympathizedBonds = new DragonBonds[playerObjects.Length];
            var count = 0;
            foreach (GameObject playerObject in playerObjects)
            {
                var dragonBonds = playerObject.GetComponent<DragonBonds>();
                if (dragonBonds &&
                    dragonBonds != this &&
                    dragonBonds._enableSympathize &&
                    dragonBonds._sympathyCircuit == _sympathyCircuit)
                {
                    sympathizedBonds[count++] = dragonBonds;
                }
            }

            var result = new DragonBonds[count];
            System.Array.Copy(sympathizedBonds, result, count);

            return result;
        }
    }
}
