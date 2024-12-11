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
    using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Bonds")]
    [RequireComponent(typeof(VRCPlayerObject), typeof(VRCEnablePersistence))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonBonds : UdonSharpBehaviour
    {
        [SerializeField]
        internal bool enableSympathy = false;
        [SerializeField]
        internal int sympathyNumber = 0;

        // Saddle
        internal Vector3 seatPosition;
        // Reins
        internal DragonReinsInputType[] selectedInput = new DragonReinsInputType[2];
        // HandType[]が扱いにくいのでint[]でやりとりする
        internal int[] throttleInputHand = new int[(int)DragonReinsInputType.count];
        internal int[] turningInputHand = new int[(int)DragonReinsInputType.count];
        internal int[] elevatorInputHand = new int[(int)DragonReinsInputType.count];
        internal bool[] invertThrust = new bool[(int)DragonReinsInputType.count];
        internal bool[] invertClimb = new bool[(int)DragonReinsInputType.count];
        internal bool[] invertStrafe = new bool[(int)DragonReinsInputType.count];
        internal bool[] invertElevator = new bool[(int)DragonReinsInputType.count];
        internal bool[] invertLadder = new bool[(int)DragonReinsInputType.count];
        internal bool[] invertAileron = new bool[(int)DragonReinsInputType.count];
        internal ReinsInputVRGrabMode[] vrGrabMode = new ReinsInputVRGrabMode[2];
        // Canopy
        internal bool canopyIndication;

        [UdonSynced] private Vector3 sync_seatPosition;
        [UdonSynced] private byte[] sync_selectedInput = new byte[2];
        [UdonSynced] private byte[] sync_throttleInputHand = new byte[(int)DragonReinsInputType.count];
        [UdonSynced] private byte[] sync_turningInputHand = new byte[(int)DragonReinsInputType.count];
        [UdonSynced] private byte[] sync_elevatorInputHand = new byte[(int)DragonReinsInputType.count];
        [UdonSynced] private bool[] sync_invertThrust = new bool[(int)DragonReinsInputType.count];
        [UdonSynced] private bool[] sync_invertClimb = new bool[(int)DragonReinsInputType.count];
        [UdonSynced] private bool[] sync_invertStrafe = new bool[(int)DragonReinsInputType.count];
        [UdonSynced] private bool[] sync_invertElevator = new bool[(int)DragonReinsInputType.count];
        [UdonSynced] private bool[] sync_invertLadder = new bool[(int)DragonReinsInputType.count];
        [UdonSynced] private bool[] sync_invertAileron = new bool[(int)DragonReinsInputType.count];
        [UdonSynced] private byte[] sync_vrGrabMode = new byte[2];
        [UdonSynced] private bool sync_canopyIndication;

        private DragonRider _rider;
        private DragonBonds[] _sympathizedBonds;

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
            sync_seatPosition = seatPosition;

            int select = Mathf.Max((int)selectedInput[0], 0);
            sync_selectedInput[0] = (byte)select;
            select = Mathf.Max((int)selectedInput[1], 0);
            sync_selectedInput[1] = (byte)select;

            for (int i = 0; i < (int)DragonReinsInputType.count; i++)
            {
                select = throttleInputHand[i];
                sync_throttleInputHand[i] = (byte)select;
                select = turningInputHand[i];
                sync_turningInputHand[i] = (byte)select;
                select = elevatorInputHand[i];
                sync_elevatorInputHand[i] = (byte)select;

                sync_invertThrust[i] = invertThrust[i];
                sync_invertClimb[i] = invertClimb[i];
                sync_invertStrafe[i] = invertStrafe[i];
                sync_invertElevator[i] = invertElevator[i];
                sync_invertLadder[i] = invertLadder[i];
                sync_invertAileron[i] = invertAileron[i];
            }

            select = (int)vrGrabMode[0];
            sync_vrGrabMode[0] = (byte)select;
            select = (int)vrGrabMode[1];
            sync_vrGrabMode[1] = (byte)select;

            sync_canopyIndication = canopyIndication;
        }

        public override void OnDeserialization()
        {
            Initialize();

            seatPosition = sync_seatPosition;

            int tmp_selectedInput = (int)sync_selectedInput[0];
            selectedInput[0] = (DragonReinsInputType)tmp_selectedInput;
            tmp_selectedInput = (int)sync_selectedInput[1];
            selectedInput[1] = (DragonReinsInputType)tmp_selectedInput;

            for (int i = 0; i < (int)DragonReinsInputType.count; i++)
            {
                throttleInputHand[i] = (int)sync_throttleInputHand[i];
                turningInputHand[i] = (int)sync_turningInputHand[i];
                elevatorInputHand[i] = (int)sync_elevatorInputHand[i];

                invertThrust[i] = sync_invertThrust[i];
                invertClimb[i] = sync_invertClimb[i];
                invertStrafe[i] = sync_invertStrafe[i];
                invertElevator[i] = sync_invertElevator[i];
                invertLadder[i] = sync_invertLadder[i];
                invertAileron[i] = sync_invertAileron[i];
            }

            vrGrabMode[0] = (ReinsInputVRGrabMode)sync_vrGrabMode[0];
            vrGrabMode[1] = (ReinsInputVRGrabMode)sync_vrGrabMode[1];

            canopyIndication = sync_canopyIndication;

            if (Networking.IsOwner(this.gameObject))
            {
                _rider._OnRemind(this);
            }
        }

        // Riderの方から一通り変更値を書き込み後、最後に実行(RequestSerialization()感覚で使う)
        internal void _Memorize()
        {
            RequestSerialization();

            if (enableSympathy)
            {
                if (_sympathizedBonds == null) { _sympathizedBonds = GetSympathizedBonds(); }

                foreach (var bonds in _sympathizedBonds)
                {
                    bonds._Sympathize(this);
                }
            }
        }

        internal void _Sympathize(DragonBonds memorizedBonds)
        {
            Initialize();

            // Saddle
            seatPosition = memorizedBonds.seatPosition;
            // Reins
            memorizedBonds.selectedInput.CopyTo(selectedInput, 0);
            memorizedBonds.throttleInputHand.CopyTo(throttleInputHand, 0);
            memorizedBonds.turningInputHand.CopyTo(turningInputHand, 0);
            memorizedBonds.elevatorInputHand.CopyTo(elevatorInputHand, 0);
            memorizedBonds.invertThrust.CopyTo(invertThrust, 0);
            memorizedBonds.invertClimb.CopyTo(invertClimb, 0);
            memorizedBonds.invertStrafe.CopyTo(invertStrafe, 0);
            memorizedBonds.invertElevator.CopyTo(invertElevator, 0);
            memorizedBonds.invertLadder.CopyTo(invertLadder, 0);
            memorizedBonds.invertAileron.CopyTo(invertAileron, 0);
            memorizedBonds.vrGrabMode.CopyTo(vrGrabMode, 0);
            // Canopy
            canopyIndication = memorizedBonds.canopyIndication;

            RequestSerialization();

            _rider._OnRemind(this);
        }

        private DragonBonds[] GetSympathizedBonds()
        {
            var playerObjects = Networking.LocalPlayer.GetPlayerObjects();
            var sympathizedBonds = new DragonBonds[playerObjects.Length];
            var count = 0;
            foreach (var playerObject in playerObjects)
            {
                var dragonBonds = playerObject.GetComponent<DragonBonds>();
                if (dragonBonds &&
                    dragonBonds != this &&
                    dragonBonds.enableSympathy &&
                    dragonBonds.sympathyNumber == sympathyNumber)
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
