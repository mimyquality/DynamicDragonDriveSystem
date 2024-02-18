/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;
    using UnityEngine.UI;
    using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;
    using TMPro;
    using UnityEngine.PlayerLoop;

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonConditioner : UdonSharpBehaviour
    {
        private const float InterlockInterval = 5.0f;

        [SerializeField] private DragonDriver target;
        [Space]
        [SerializeField] private Button btnSetOwner;
        [Space]
        [SerializeField] private Slider sldAcceleration;
        [SerializeField] private TextMeshProUGUI txtAcceleration;
        [SerializeField] private Slider sldMaxSpeed;
        [SerializeField] private TextMeshProUGUI txtMaxSpeed;
        [SerializeField] private Slider sldMaxWalkSpeed;
        [SerializeField] private TextMeshProUGUI txtMaxWalkSpeed;
        [SerializeField] private Slider sldHoveringSpeedThreshold;
        [SerializeField] private TextMeshProUGUI txtHoveringSpeedThreshold;
        [SerializeField] private Slider sldUpdownSpeed;
        [SerializeField] private TextMeshProUGUI txtUpdownSpeed;
        [SerializeField] private Slider sldRollSpeed;
        [SerializeField] private TextMeshProUGUI txtRollSpeed;
        [SerializeField] private Slider sldRollToTurnRatio;
        [SerializeField] private TextMeshProUGUI txtRollToTurnRatio;
        [SerializeField] private Slider sldNoseRotateSpeed;
        [SerializeField] private TextMeshProUGUI txtNoseRotateSpeed;
        [SerializeField] private Slider sldMaxNosePitch;
        [SerializeField] private TextMeshProUGUI txtMaxNosePitch;
        [SerializeField] private Slider sldMaxNoseYaw;
        [SerializeField] private TextMeshProUGUI txtMaxNoseYaw;
        [SerializeField] private Slider sldJumpImpulse;
        [SerializeField] private TextMeshProUGUI txtJumpImpulse;

        private bool _interlockSetOwner;

        [UdonSynced, FieldChangeCallback(nameof(Acceleration))]
        private float _acceleration;
        private float Acceleration
        {
            get => _acceleration;
            set
            {
                Initialize();
                SetAcceleration(value);
            }
        }
        private void SetAcceleration(float value)
        {
            if (_initialized && target) { target.Acceleration = value; }
            sldAcceleration.SetValueWithoutNotify(value);
            txtAcceleration.text = value.ToString("F1");
            _acceleration = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(MaxSpeed))]
        private float _maxSpeed;
        private float MaxSpeed
        {
            get => _maxSpeed;
            set
            {
                Initialize();
                SetMaxSpeed(value);
            }
        }
        private void SetMaxSpeed(float value)
        {
            if (_initialized && target) { target.MaxSpeed = value; }
            sldMaxSpeed.SetValueWithoutNotify(value);
            txtMaxSpeed.text = value.ToString("F1");
            _maxSpeed = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(MaxWalkSpeed))]
        private float _maxWalkSpeed;
        private float MaxWalkSpeed
        {
            get => _maxWalkSpeed;
            set
            {
                Initialize();
                SetMaxWalkSpeed(value);
            }
        }
        private void SetMaxWalkSpeed(float value)
        {
            if (_initialized && target) { target.MaxWalkSpeed = value; }
            sldMaxWalkSpeed.SetValueWithoutNotify(value);
            txtMaxWalkSpeed.text = value.ToString("F1");
            _maxWalkSpeed = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(HoveringSpeedThreshold))]
        private float _hoveringSpeedThreshold;
        private float HoveringSpeedThreshold
        {
            get => _hoveringSpeedThreshold;
            set
            {
                Initialize();
                SetHoveringSpeedThreshold(value);
            }
        }
        private void SetHoveringSpeedThreshold(float value)
        {
            if (_initialized && target) { target.HoveringSpeedThreshold = value; }
            sldHoveringSpeedThreshold.SetValueWithoutNotify(value);
            txtHoveringSpeedThreshold.text = value.ToString("F1");
            _hoveringSpeedThreshold = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(UpdownSpeed))]
        private float _updownSpeed;
        private float UpdownSpeed
        {
            get => _updownSpeed;
            set
            {
                Initialize();
                SetUpdownSpeed(value);
            }
        }
        private void SetUpdownSpeed(float value)
        {
            if (_initialized && target) { target.UpdownSpeed = value; }
            sldUpdownSpeed.SetValueWithoutNotify(value);
            txtUpdownSpeed.text = value.ToString("F1");
            _updownSpeed = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(RollSpeed))]
        private float _rollSpeed;
        private float RollSpeed
        {
            get => _rollSpeed;
            set
            {
                Initialize();
                SetRollSpeed(value);
            }
        }
        private void SetRollSpeed(float value)
        {
            if (_initialized && target) { target.RollSpeed = value; }
            sldRollSpeed.SetValueWithoutNotify(value);
            txtRollSpeed.text = value.ToString("F1");
            _rollSpeed = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(RollToTurnRatio))]
        private float _rollToTurnRatio;
        private float RollToTurnRatio
        {
            get => _rollToTurnRatio;
            set
            {
                Initialize();
                SetRollToTurnRatio(value);
            }
        }
        private void SetRollToTurnRatio(float value)
        {
            if (_initialized && target) { target.RollToTurnRatio = value; }
            sldRollToTurnRatio.SetValueWithoutNotify(value);
            txtRollToTurnRatio.text = value.ToString("F2");
            _rollToTurnRatio = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(NoseRotateSpeed))]
        private float _noseRotateSpeed;
        private float NoseRotateSpeed
        {
            get => _noseRotateSpeed;
            set
            {
                Initialize();
                SetNoseRotateSpeed(value);
            }
        }
        private void SetNoseRotateSpeed(float value)
        {
            if (_initialized && target) { target.NoseRotateSpeed = value; }
            sldNoseRotateSpeed.SetValueWithoutNotify(value);
            txtNoseRotateSpeed.text = value.ToString("F1");
            _noseRotateSpeed = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(MaxNosePitch))]
        private float _maxNosePitch;
        private float MaxNosePitch
        {
            get => _maxNosePitch;
            set
            {
                Initialize();
                SetMaxNosePitch(value);
            }
        }
        private void SetMaxNosePitch(float value)
        {
            if (_initialized && target) { target.MaxNosePitch = value; }
            sldMaxNosePitch.SetValueWithoutNotify(value);
            txtMaxNosePitch.text = value.ToString("F1");
            _maxNosePitch = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(MaxNoseYaw))]
        private float _maxNoseYaw;
        private float MaxNoseYaw
        {
            get => _maxNoseYaw;
            set
            {
                Initialize();
                SetMaxNoseYaw(value);
            }
        }
        private void SetMaxNoseYaw(float value)
        {
            if (_initialized && target) { target.MaxNoseYaw = value; }
            sldMaxNoseYaw.SetValueWithoutNotify(value);
            txtMaxNoseYaw.text = value.ToString("F1");
            _maxNoseYaw = value;
        }

        [UdonSynced, FieldChangeCallback(nameof(JumpImpulse))]
        private float _jumpImpulse;
        private float JumpImpulse
        {
            get => _jumpImpulse;
            set
            {
                Initialize();
                SetJumpImpulse(value);
            }
        }
        private void SetJumpImpulse(float value)
        {
            if (_initialized && target) { target.JumpImpulse = value; }
            sldJumpImpulse.SetValueWithoutNotify(value);
            txtJumpImpulse.text = value.ToString("F1");
            _jumpImpulse = value;
        }

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            if (target)
            {
                SetAcceleration(target.Acceleration);
                SetMaxSpeed(target.MaxSpeed);
                SetMaxWalkSpeed(target.MaxWalkSpeed);
                SetHoveringSpeedThreshold(target.HoveringSpeedThreshold);
                SetUpdownSpeed(target.UpdownSpeed);
                SetRollSpeed(target.RollSpeed);
                SetRollToTurnRatio(target.RollToTurnRatio);
                SetNoseRotateSpeed(target.NoseRotateSpeed);
                SetMaxNosePitch(target.MaxNosePitch);
                SetMaxNoseYaw(target.MaxNoseYaw);
                SetJumpImpulse(target.JumpImpulse);
            }

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
            btnSetOwner.interactable = !Networking.IsOwner(this.gameObject);
            ToggleInteractable(target && Networking.IsOwner(this.gameObject));
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            _interlockSetOwner = true;
            btnSetOwner.interactable = false;
            SendCustomEventDelayedSeconds(nameof(_UnlockSetOwner), InterlockInterval);

            ToggleInteractable(Networking.IsOwner(this.gameObject));
        }
        public void _UnlockSetOwner()
        {
            _interlockSetOwner = false;
            btnSetOwner.interactable = !Networking.IsOwner(this.gameObject);
        }

        /******************************
         uGUI用イベント
         ******************************/
        public void _SetOwner()
        {
            if (_interlockSetOwner) { return; }

            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }

        public void _ChangeAcceleration()
        {
            Acceleration = sldAcceleration.value;
            RequestSerialization();
        }

        public void _ChangeMaxSpeed()
        {
            MaxSpeed = sldMaxSpeed.value;
            RequestSerialization();
        }

        public void _ChangeMaxWalkSpeed()
        {
            MaxWalkSpeed = sldMaxWalkSpeed.value;
            RequestSerialization();
        }

        public void _ChangeHoveringSpeedThreshold()
        {
            HoveringSpeedThreshold = sldHoveringSpeedThreshold.value;
            RequestSerialization();
        }

        public void _ChangeUpdownSpeed()
        {
            UpdownSpeed = sldUpdownSpeed.value;
            RequestSerialization();
        }

        public void _ChangeRollSpeed()
        {
            RollSpeed = sldRollSpeed.value;
            RequestSerialization();
        }

        public void _ChangeRollToTurnRatio()
        {
            RollToTurnRatio = sldRollToTurnRatio.value;
            RequestSerialization();
        }

        public void _ChangeNoseRotateSpeed()
        {
            NoseRotateSpeed = sldNoseRotateSpeed.value;
            RequestSerialization();
        }

        public void _ChangeMaxNosePitch()
        {
            MaxNosePitch = sldMaxNosePitch.value;
            RequestSerialization();
        }

        public void _ChangeMaxNoseYaw()
        {
            MaxNoseYaw = sldMaxNoseYaw.value;
            RequestSerialization();
        }

        public void _ChangeJumpImpulse()
        {
            JumpImpulse = sldJumpImpulse.value;
            RequestSerialization();
        }

        /******************************
         内部処理用メソッド
         ******************************/
        private void ToggleInteractable(bool interactable)
        {
            sldAcceleration.interactable = interactable;
            sldMaxSpeed.interactable = interactable;
            sldMaxWalkSpeed.interactable = interactable;
            sldHoveringSpeedThreshold.interactable = interactable;
            sldUpdownSpeed.interactable = interactable;
            sldRollSpeed.interactable = interactable;
            sldRollToTurnRatio.interactable = interactable;
            sldNoseRotateSpeed.interactable = interactable;
            sldMaxNosePitch.interactable = interactable;
            sldMaxNoseYaw.interactable = interactable;
            sldJumpImpulse.interactable = interactable;
        }
    }
}
