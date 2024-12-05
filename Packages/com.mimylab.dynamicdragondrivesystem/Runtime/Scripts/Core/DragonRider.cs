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
    using VRC.Udon.Common;

    public enum DragonRiderToggleInstruction
    {
        SeatAdjust,
        InvertThrust,
        InvertClimb,
        InvertStrafe,
        InvertElevator,
        InvertLadder,
        InvertAileron,
        ShowCanopy,
    }

    public enum DragonRiderSelectInstruction
    {
        ReinsInput,
        ThrottleInputHand,
        TurningInputHand,
        ElevatorInputHand,
        VRGrabMode,
    }

    public enum DragonRiderVolumeInstruction
    {
        None
    }

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Rider")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonRider : UdonSharpBehaviour
    {
        internal DragonDriver driver;
        internal DragonSaddle saddle;
        internal DragonReins reins;
        internal DragonActor actor;

        [SerializeField]
        private GameObject _menu;
        [SerializeField]
        private GameObject _canopy;

        private DragonBonds _bonds;
        private RiderInstructToggle[] _toggles;
        private DragonRiderToggleInstruction[] _togglesInstruction;
        private RiderInstructSelect[] _selects;
        private DragonRiderSelectInstruction[] _selectsInstruction;
        private RiderInstructVolume[] _volumes;
        private DragonRiderVolumeInstruction[] _volumesInstruction;

        // Saddle
        private bool _isPilot = false;
        private bool _seatAdjust = false;

        // Seat
        private bool _isRide = false;
        private int _rideCount = 0;
        private bool _isMount = false;
        private int _mountCount = 0;

        // Canopy
        private bool _canopyIndication = false;

        // Actor渡し用
        public bool IsPilot { get => _isPilot; }
        public bool IsRide { get => _isRide; }
        public bool IsMount { get => _isMount; }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            _toggles = _menu.GetComponentsInChildren<RiderInstructToggle>(true);
            _togglesInstruction = new DragonRiderToggleInstruction[_toggles.Length];
            for (int i = 0; i < _toggles.Length; i++)
            {
                _toggles[i].rider = this;
                _togglesInstruction[i] = _toggles[i].Instruction;
            }
            _selects = _menu.GetComponentsInChildren<RiderInstructSelect>(true);
            _selectsInstruction = new DragonRiderSelectInstruction[_selects.Length];
            for (int j = 0; j < _selects.Length; j++)
            {
                _selects[j].rider = this;
                _selectsInstruction[j] = _selects[j].Instruction;
            }
            _volumes = _menu.GetComponentsInChildren<RiderInstructVolume>(true);
            _volumesInstruction = new DragonRiderVolumeInstruction[_volumes.Length];
            for (int k = 0; k < _volumes.Length; k++)
            {
                _volumes[k].rider = this;
                _volumesInstruction[k] = _volumes[k].Instruction;
            }

            var categorizers = _menu.GetComponentsInChildren<RiderSubCategory>(true);
            foreach (var categorizer in categorizers)
            {
                var reinsInput = categorizer.reinsInput;
                var toggles = categorizer.GetComponentsInChildren<RiderInstructToggle>(true);
                foreach (var toggle in toggles)
                {
                    toggle.targetReinsInput = reinsInput;
                }
                var selects = categorizer.GetComponentsInChildren<RiderInstructSelect>(true);
                foreach (var select in selects)
                {
                    select.targetReinsInput = reinsInput;
                }
                var volumes = categorizer.GetComponentsInChildren<RiderInstructVolume>(true);
                foreach (var volume in volumes)
                {
                    volume.targetReinsInput = reinsInput;
                }
            }            

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            _menu.SetActive(false);
        }

        /******************************
         Saddle からのイベント
         ******************************/
        internal void _OnSaddleRided()
        {
            Initialize();

            Networking.SetOwner(Networking.LocalPlayer, driver.gameObject);
            if (actor) { Networking.SetOwner(Networking.LocalPlayer, actor.gameObject); }
            _isPilot = true;
            _menu.SetActive(_isPilot);
            driver.IsDrive = _isPilot;
            SeatAdjust(false);
            ShowCanopy(_canopyIndication);

            FeedbackAll();
        }

        internal void _OnSaddleExited()
        {
            _isPilot = false;
            _menu.SetActive(_isPilot);
            driver.IsDrive = _isPilot;
            SeatAdjust(false);
            ShowCanopy(_canopyIndication);

            FeedbackAll();
        }

        /******************************
         Bonds からのイベント
         ******************************/
        internal void _OnRemind(DragonBonds bonds)
        {
            Initialize();

        }

        /******************************
         UI からのイベント
         ******************************/
        internal void _OnToggleChanged(RiderInstructToggle toggler)
        {
            var instruction = toggler.Instruction;
            switch (instruction)
            {
                case DragonRiderToggleInstruction.SeatAdjust: SeatAdjust(toggler); break;
                case DragonRiderToggleInstruction.InvertThrust: InvertThrust(toggler); break;
                case DragonRiderToggleInstruction.InvertClimb: InvertClimb(toggler); break;
                case DragonRiderToggleInstruction.InvertStrafe: InvertStrafe(toggler); break;
                case DragonRiderToggleInstruction.InvertElevator: InvertElevator(toggler); break;
                case DragonRiderToggleInstruction.InvertLadder: InvertLadder(toggler); break;
                case DragonRiderToggleInstruction.InvertAileron: InvertAileron(toggler); break;
                case DragonRiderToggleInstruction.ShowCanopy: ShowCanopy(toggler); break;
            }

            FeedbackToggles(instruction);
        }

        internal void _OnSelectChanged(RiderInstructSelect selector)
        {
            var instruction = selector.Instruction;
            switch (instruction)
            {
                case DragonRiderSelectInstruction.ReinsInput: SelectReinsInput(selector); break;
                case DragonRiderSelectInstruction.ThrottleInputHand: ThrottleInputHand(selector); break;
                case DragonRiderSelectInstruction.TurningInputHand: TurnInputHand(selector); break;
                case DragonRiderSelectInstruction.ElevatorInputHand: ElevatorInputHand(selector); break;
                case DragonRiderSelectInstruction.VRGrabMode: VRGrabMode(selector); break;
            }

            FeedbackSelects(instruction);
        }

        internal void _OnVolumeChanged(RiderInstructVolume volumer)
        {
            // 未使用
        }

        /******************************
         その他イベント
         ******************************/
        internal void _OnSeatRided(bool value)
        {
            _rideCount += value ? 1 : -1;
            _rideCount = Mathf.Max(_rideCount, 0);

            _isRide = _rideCount > 0;
        }

        internal void _OnSeatMounted(bool value)
        {
            _mountCount += value ? 1 : -1;
            _mountCount = Mathf.Max(_mountCount, 0);

            _isMount = _mountCount > 0;
        }

        internal void _Bond(DragonBonds bonds)
        {
            _bonds = bonds;
        }

        /******************************
         private メソッド
         ******************************/
        private void SeatAdjust(RiderInstructToggle toggler)
        {
            SeatAdjust(toggler.IsOn);
        }
        private void SeatAdjust(bool value)
        {
            _seatAdjust = value;
            saddle.EnableAdjust = _seatAdjust & _isPilot;
            reins.InputEnabled = !_seatAdjust & _isPilot;
        }

        private void SelectReinsInput(RiderInstructSelect selector)
        {
            reins.SelectedInput = (DragonReinsInputType)selector.Select;
        }

        private void InvertThrust(RiderInstructToggle toggler)
        {
            var reinsInput = reins._GetInput(toggler.targetReinsInput);
            if (reinsInput) { reinsInput.ThrustIsInvert = toggler.IsOn; }
        }

        private void InvertClimb(RiderInstructToggle toggler)
        {
            var reinsInput = reins._GetInput(toggler.targetReinsInput);
            if (reinsInput) { reinsInput.ClimbIsInvert = toggler.IsOn; }
        }

        private void InvertStrafe(RiderInstructToggle toggler)
        {
            var reinsInput = reins._GetInput(toggler.targetReinsInput);
            if (reinsInput) { reinsInput.StrafeIsInvert = toggler.IsOn; }
        }

        private void InvertElevator(RiderInstructToggle toggler)
        {
            var reinsInput = reins._GetInput(toggler.targetReinsInput);
            if (reinsInput) { reinsInput.ElevatorIsInvert = toggler.IsOn; }
        }

        private void InvertLadder(RiderInstructToggle toggler)
        {

            var reinsInput = reins._GetInput(toggler.targetReinsInput);
            if (reinsInput) { reinsInput.LadderIsInvert = toggler.IsOn; }
        }

        private void InvertAileron(RiderInstructToggle toggler)
        {

            var reinsInput = reins._GetInput(toggler.targetReinsInput);
            if (reinsInput) { reinsInput.AileronIsInvert = toggler.IsOn; }
        }

        private void ThrottleInputHand(RiderInstructSelect selector)
        {
            var reinsInput = reins._GetInput(selector.targetReinsInput);
            if (reinsInput) { reinsInput.ThrottleInputHand = IntToHandType(selector.Select); }
        }

        private void TurnInputHand(RiderInstructSelect selector)
        {
            var reinsInput = reins._GetInput(selector.targetReinsInput);
            if (reinsInput) { reinsInput.TurnInputHand = IntToHandType(selector.Select); }
        }

        private void ElevatorInputHand(RiderInstructSelect selector)
        {
            var reinsInput = reins._GetInput(selector.targetReinsInput);
            if (reinsInput) { reinsInput.ElevatorInputHand = IntToHandType(selector.Select); }
        }

        private void VRGrabMode(RiderInstructSelect selector)
        {
            switch (selector.targetReinsInput)
            {
                case DragonReinsInputType.VRHands:
                    var vrHands = reins.vrHands;
                    if (vrHands) { vrHands.VRGrabMode = (ReinsInputVRGrabMode)selector.Select; }
                    break;
                case DragonReinsInputType.VRHands2:
                    var vrHands2 = reins.vrHands2;
                    if (vrHands2) { vrHands2.VRGrabMode = (ReinsInputVRGrabMode)selector.Select; }
                    break;
            }
        }

        private void ShowCanopy(RiderInstructToggle toggler)
        {
            ShowCanopy(toggler.IsOn);
        }
        private void ShowCanopy(bool value)
        {
            _canopyIndication = value;
            if (_canopy) { _canopy.SetActive(_canopyIndication & _isPilot); }
        }

        private HandType IntToHandType(int value)
        {
            return value == 1 ? HandType.LEFT : HandType.RIGHT;
        }

        /******************************
         Feedback 関連
         ******************************/
        private void FeedbackAll()
        {
            FeedbackToggles();
            FeedbackSelects();
            //FeedbackVolumes();
        }

        private void FeedbackToggles()
        {
            for (int i = 0; i < _toggles.Length; i++)
            {
                FeedbackToggle(i);
            }
        }
        private void FeedbackToggles(DragonRiderToggleInstruction instruction)
        {
            var index = 0;
            while ((index = System.Array.IndexOf(_togglesInstruction, instruction, index)) > -1)
            {
                FeedbackToggle(index++);

                if (index >= _togglesInstruction.Length) { break; }
            }
        }
        private void FeedbackToggle(int index)
        {
            var reinsInput = reins._GetInput(_toggles[index].targetReinsInput);
            switch (_togglesInstruction[index])
            {
                case DragonRiderToggleInstruction.SeatAdjust:
                    _toggles[index]._OnValueChanged(_seatAdjust);
                    break;
                case DragonRiderToggleInstruction.InvertThrust:
                    if (reinsInput) { _toggles[index]._OnValueChanged(reinsInput.ThrustIsInvert); }
                    break;
                case DragonRiderToggleInstruction.InvertClimb:
                    if (reinsInput) { _toggles[index]._OnValueChanged(reinsInput.ClimbIsInvert); }
                    break;
                case DragonRiderToggleInstruction.InvertStrafe:
                    if (reinsInput) { _toggles[index]._OnValueChanged(reinsInput.StrafeIsInvert); }
                    break;
                case DragonRiderToggleInstruction.InvertElevator:
                    if (reinsInput) { _toggles[index]._OnValueChanged(reinsInput.ElevatorIsInvert); }
                    break;
                case DragonRiderToggleInstruction.InvertLadder:
                    if (reinsInput) { _toggles[index]._OnValueChanged(reinsInput.LadderIsInvert); }
                    break;
                case DragonRiderToggleInstruction.InvertAileron:
                    if (reinsInput) { _toggles[index]._OnValueChanged(reinsInput.AileronIsInvert); }
                    break;
                case DragonRiderToggleInstruction.ShowCanopy:
                    _toggles[index]._OnValueChanged(_canopyIndication);
                    break;
            }
        }

        private void FeedbackSelects()
        {
            for (int i = 0; i < _selects.Length; i++)
            {
                FeedbackSelect(i);
            }
        }
        private void FeedbackSelects(DragonRiderSelectInstruction instruction)
        {
            var index = 0;
            while ((index = System.Array.IndexOf(_selectsInstruction, instruction, index)) > -1)
            {
                FeedbackSelect(index++);

                if (index >= _selectsInstruction.Length) { break; }
            }
        }
        private void FeedbackSelect(int index)
        {
            var reinsInput = reins._GetInput(_selects[index].targetReinsInput);
            switch (_selectsInstruction[index])
            {
                case DragonRiderSelectInstruction.ReinsInput:
                    _selects[index]._OnValueChanged((int)reins.SelectedInput);
                    break;
                case DragonRiderSelectInstruction.ThrottleInputHand:
                    if (reinsInput) { _selects[index]._OnValueChanged((int)reinsInput.ThrottleInputHand); }
                    break;
                case DragonRiderSelectInstruction.TurningInputHand:
                    if (reinsInput) { _selects[index]._OnValueChanged((int)reinsInput.TurnInputHand); }
                    break;
                case DragonRiderSelectInstruction.ElevatorInputHand:
                    if (reinsInput) { _selects[index]._OnValueChanged((int)reinsInput.ElevatorInputHand); }
                    break;
                case DragonRiderSelectInstruction.VRGrabMode:
                    switch (_selects[index].targetReinsInput)
                    {
                        case DragonReinsInputType.VRHands:
                            var vrHands = reins.vrHands;
                            if (vrHands) { _selects[index]._OnValueChanged((int)vrHands.VRGrabMode); }
                            break;
                        case DragonReinsInputType.VRHands2:
                            var vrHands2 = reins.vrHands2;
                            if (vrHands2) { _selects[index]._OnValueChanged((int)vrHands2.VRGrabMode); }
                            break;
                    }
                    break;
            }
        }

        private void FeedbackVolumes()
        {
            for (int i = 0; i < _volumes.Length; i++)
            {
                FeedbackVolume(i);
            }
        }
        private void FeedbackVolumes(DragonRiderVolumeInstruction instruction)
        {
            var index = 0;
            while ((index = System.Array.IndexOf(_volumesInstruction, instruction, index)) > -1)
            {
                FeedbackVolume(index++);

                if (index >= _volumesInstruction.Length) { break; }
            }
        }
        private void FeedbackVolume(int index)
        {
            // 未使用
        }
    }
}
