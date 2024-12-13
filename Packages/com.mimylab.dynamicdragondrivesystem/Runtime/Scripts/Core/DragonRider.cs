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
        private RiderInstructSelectable[] _selectables;
        private DragonRiderSelectInstruction[] _selectablesInstruction;
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

        //Reins
        private bool _isInitialSelect = true;

        // Canopy
        private bool _canopyIndication = false;

        // Actor渡し用
        public bool IsPilot { get => _isPilot; }
        public bool IsRide { get => _isRide; }
        public bool IsMount { get => _isMount; }

        public static HandType IntToHandType(int value)
        {
            return value == 1 ? HandType.LEFT : HandType.RIGHT;
        }

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
            _selectables = _menu.GetComponentsInChildren<RiderInstructSelectable>(true);
            _selectablesInstruction = new DragonRiderSelectInstruction[_selectables.Length];
            for (int k = 0; k < _selectables.Length; k++)
            {
                _selectables[k].rider = this;
                _selectablesInstruction[k] = _selectables[k].Instruction;
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

        public override void OnInputMethodChanged(VRCInputMethod inputMethod)
        {
            int inputMethodInt = (int)inputMethod;
            switch (inputMethodInt)
            {
                case (int)VRCInputMethod.Keyboard:
                case (int)VRCInputMethod.Mouse:
                    reins.SetChangeableInput(DragonReinsInputType.Keyboard);
                    reins.SetChangeableInput(DragonReinsInputType.Gaze);
                    if (_isInitialSelect) { reins.SelectedInput = DragonReinsInputType.Keyboard; }
                    break;
                case (int)VRCInputMethod.Controller:
                    reins.SetChangeableInput(DragonReinsInputType.Thumbsticks);
                    if (_isInitialSelect) { reins.SelectedInput = DragonReinsInputType.Thumbsticks; }
                    break;
                case (int)VRCInputMethod.Vive:
                    reins.SetChangeableInput(DragonReinsInputType.Legacy);
                    reins.SetChangeableInput(DragonReinsInputType.VRHands);
                    reins.SetChangeableInput(DragonReinsInputType.VRHands2);
                    if (_isInitialSelect) { reins.SelectedInput = DragonReinsInputType.Legacy; }
                    break;
                case (int)VRCInputMethod.Oculus:
                case (int)VRCInputMethod.ViveXr:
                case (int)VRCInputMethod.Index:
                case (int)VRCInputMethod.HPMotionController:
                case (int)VRCInputMethod.QuestHands:
                case (int)VRCInputMethod.OpenXRGeneric:
                case (int)VRCInputMethod.Pico:
                case (int)VRCInputMethod.SteamVR2:
                    reins.SetChangeableInput(DragonReinsInputType.Thumbsticks);
                    reins.SetChangeableInput(DragonReinsInputType.VRHands);
                    reins.SetChangeableInput(DragonReinsInputType.VRHands2);
                    if (_isInitialSelect) { reins.SelectedInput = DragonReinsInputType.Thumbsticks; }
                    break;
                case (int)VRCInputMethod.Touch:
                    reins.SetChangeableInput(DragonReinsInputType.Gaze);
                    if (_isInitialSelect) { reins.SelectedInput = DragonReinsInputType.Gaze; }
                    break;
                default:
                    reins.SetChangeableInput(DragonReinsInputType.Thumbsticks);
                    if (_isInitialSelect) { reins.SelectedInput = DragonReinsInputType.Thumbsticks; }
                    break;
            }

            if (_isInitialSelect) { FeedbackSelects(DragonRiderSelectInstruction.ReinsInput); }
            FeedbackSelectables(DragonRiderSelectInstruction.ReinsInput);

            _isInitialSelect = false;
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

            if (_bonds)
            {
                _bonds.seatPosition = saddle.AdjustPoint;
                _bonds._Remember();
            }
        }

        /******************************
         Bonds からのイベント
         ******************************/
        internal void _Bond(DragonBonds bonds)
        {
            Initialize();

            _bonds = bonds;

            // Saddle
            _bonds.seatPosition = saddle.AdjustPoint;

            // Reins
            var isVR = Networking.LocalPlayer.IsUserInVR();
            _bonds.selectedInput[isVR ? 0 : 1] = reins.SelectedInput;

            for (int i = 0; i < (int)DragonReinsInputType.Count; i++)
            {
                var reinsInput = reins._GetInput((DragonReinsInputType)i);
                if (!reinsInput) { continue; }

                _bonds.throttleInputHand[i] = (int)reinsInput.ThrottleInputHand;
                _bonds.turningInputHand[i] = (int)reinsInput.TurnInputHand;
                _bonds.elevatorInputHand[i] = (int)reinsInput.ElevatorInputHand;

                _bonds.invertThrust[i] = reinsInput.ThrustIsInvert;
                _bonds.invertClimb[i] = reinsInput.ClimbIsInvert;
                _bonds.invertStrafe[i] = reinsInput.StrafeIsInvert;
                _bonds.invertElevator[i] = reinsInput.ElevatorIsInvert;
                _bonds.invertLadder[i] = reinsInput.LadderIsInvert;
                _bonds.invertAileron[i] = reinsInput.AileronIsInvert;

                switch ((DragonReinsInputType)i)
                {
                    case DragonReinsInputType.VRHands:
                        var vrHands = reins.vrHands;
                        if (vrHands) { _bonds.vrGrabMode[0] = vrHands.VRGrabMode; }
                        break;
                    case DragonReinsInputType.VRHands2:
                        var vrHands2 = reins.vrHands2;
                        if (vrHands2) { _bonds.vrGrabMode[1] = vrHands2.VRGrabMode; }
                        break;
                }
            }

            // Canopy
            _bonds.canopyIndication = _canopyIndication;
        }

        internal void _OnRemind(DragonBonds bonds)
        {
            Initialize();

            // Saddle
            saddle._SetLocalAdjustPoint(bonds.seatPosition);

            // Reins
            var isVR = Networking.LocalPlayer.IsUserInVR();
            reins.SelectedInput = bonds.selectedInput[isVR ? 0 : 1];
            _isInitialSelect = false;

            var throttleInputHand = bonds.throttleInputHand;
            var turningInputHand = bonds.turningInputHand;
            var elevatorInputHand = bonds.elevatorInputHand;
            var invertThrust = bonds.invertThrust;
            var invertClimb = bonds.invertClimb;
            var invertStrafe = bonds.invertStrafe;
            var invertElevator = bonds.invertElevator;
            var invertLadder = bonds.invertLadder;
            var invertAileron = bonds.invertAileron;
            for (int i = 0; i < (int)DragonReinsInputType.Count; i++)
            {
                var reinsInput = reins._GetInput((DragonReinsInputType)i);
                if (!reinsInput) { continue; }

                reinsInput.ThrottleInputHand = IntToHandType(throttleInputHand[i]);
                reinsInput.TurnInputHand = IntToHandType(turningInputHand[i]);
                reinsInput.ElevatorInputHand = IntToHandType(elevatorInputHand[i]);

                reinsInput.ThrustIsInvert = invertThrust[i];
                reinsInput.ClimbIsInvert = invertClimb[i];
                reinsInput.StrafeIsInvert = invertStrafe[i];
                reinsInput.ElevatorIsInvert = invertElevator[i];
                reinsInput.LadderIsInvert = invertLadder[i];
                reinsInput.AileronIsInvert = invertAileron[i];
            }

            var vrHands = reins.vrHands;
            if (vrHands) { vrHands.VRGrabMode = bonds.vrGrabMode[0]; }
            var vrHands2 = reins.vrHands2;
            if (vrHands2) { vrHands2.VRGrabMode = bonds.vrGrabMode[1]; }

            // Canopy
            ShowCanopy(bonds.canopyIndication);

            FeedbackAll();
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

            if (_bonds) { _bonds._Remember(); }
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

            if (_bonds) { _bonds._Remember(); }
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

        /******************************
         private メソッド
         ******************************/
        private void SeatAdjust(RiderInstructToggle toggler)
        {
            var isOn = toggler.IsOn;
            SeatAdjust(isOn);

            if (!isOn && _bonds)
            {
                _bonds.seatPosition = saddle.AdjustPoint;
            }
        }
        private void SeatAdjust(bool value)
        {
            _seatAdjust = value;
            saddle.EnableAdjust = _seatAdjust & _isPilot;
            reins.InputEnabled = !_seatAdjust & _isPilot;
        }

        private void SelectReinsInput(RiderInstructSelect selector)
        {
            var select = (DragonReinsInputType)selector.Select;
            reins.SelectedInput = select;

            if (_bonds)
            {
                var isVR = Networking.LocalPlayer.IsUserInVR();
                _bonds.selectedInput[isVR ? 0 : 1] = select;
            }
        }

        private void InvertThrust(RiderInstructToggle toggler)
        {
            var invert = toggler.IsOn;
            var target = toggler.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.ThrustIsInvert = invert; }

            if (_bonds) { _bonds.invertThrust[(int)target] = invert; }
        }

        private void InvertClimb(RiderInstructToggle toggler)
        {
            var invert = toggler.IsOn;
            var target = toggler.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.ClimbIsInvert = invert; }

            if (_bonds) { _bonds.invertClimb[(int)target] = invert; }
        }

        private void InvertStrafe(RiderInstructToggle toggler)
        {
            var invert = toggler.IsOn;
            var target = toggler.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.StrafeIsInvert = invert; }

            if (_bonds) { _bonds.invertStrafe[(int)target] = invert; }
        }

        private void InvertElevator(RiderInstructToggle toggler)
        {
            var invert = toggler.IsOn;
            var target = toggler.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.ElevatorIsInvert = invert; }

            if (_bonds) { _bonds.invertElevator[(int)target] = invert; }
        }

        private void InvertLadder(RiderInstructToggle toggler)
        {
            var invert = toggler.IsOn;
            var target = toggler.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.LadderIsInvert = invert; }

            if (_bonds) { _bonds.invertLadder[(int)target] = invert; }
        }

        private void InvertAileron(RiderInstructToggle toggler)
        {
            var invert = toggler.IsOn;
            var target = toggler.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.AileronIsInvert = invert; }

            if (_bonds) { _bonds.invertAileron[(int)target] = invert; }
        }

        private void ThrottleInputHand(RiderInstructSelect selector)
        {
            var select = selector.Select;
            var target = selector.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.ThrottleInputHand = IntToHandType(select); }

            if (_bonds) { _bonds.throttleInputHand[(int)target] = select; }
        }

        private void TurnInputHand(RiderInstructSelect selector)
        {
            var select = selector.Select;
            var target = selector.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.TurnInputHand = IntToHandType(select); }

            if (_bonds) { _bonds.turningInputHand[(int)target] = select; }
        }

        private void ElevatorInputHand(RiderInstructSelect selector)
        {
            var select = selector.Select;
            var target = selector.targetReinsInput;
            var reinsInput = reins._GetInput(target);
            if (reinsInput) { reinsInput.ElevatorInputHand = IntToHandType(select); }

            if (_bonds) { _bonds.elevatorInputHand[(int)target] = select; }
        }

        private void VRGrabMode(RiderInstructSelect selector)
        {
            var select = (ReinsInputVRGrabMode)selector.Select;
            switch (selector.targetReinsInput)
            {
                case DragonReinsInputType.VRHands:
                    var vrHands = reins.vrHands;
                    if (vrHands) { vrHands.VRGrabMode = select; }
                    if (_bonds) { _bonds.vrGrabMode[0] = select; }
                    break;
                case DragonReinsInputType.VRHands2:
                    var vrHands2 = reins.vrHands2;
                    if (vrHands2) { vrHands2.VRGrabMode = select; }
                    if (_bonds) { _bonds.vrGrabMode[1] = select; }
                    break;
            }
        }

        private void ShowCanopy(RiderInstructToggle toggler)
        {
            ShowCanopy(toggler.IsOn);

            if (_bonds) { _bonds.canopyIndication = _canopyIndication; }
        }
        private void ShowCanopy(bool value)
        {
            _canopyIndication = value;
            if (_canopy) { _canopy.SetActive(_canopyIndication & _isPilot); }
        }

        /******************************
         Feedback 関連
         ******************************/
        private void FeedbackAll()
        {
            FeedbackToggles();
            FeedbackSelects();
            FeedbackSelectables();
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

        private void FeedbackSelectables()
        {
            for (int i = 0; i < _selectables.Length; i++)
            {
                FeedbackSelectable(i);
            }
        }
        private void FeedbackSelectables(DragonRiderSelectInstruction instruction)
        {
            var index = 0;
            while ((index = System.Array.IndexOf(_selectablesInstruction, instruction, index)) > -1)
            {
                FeedbackSelectable(index++);

                if (index >= _selectablesInstruction.Length) { break; }
            }
        }
        private void FeedbackSelectable(int index)
        {
            switch (_selectablesInstruction[index])
            {
                case DragonRiderSelectInstruction.ReinsInput:
                    _selectables[index]._OnValueChanged(reins.ChangeableInput);
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
