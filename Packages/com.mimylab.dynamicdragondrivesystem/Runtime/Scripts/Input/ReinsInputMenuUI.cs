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
    //using VRC.Udon;
    using VRC.Udon.Common;

    [AddComponentMenu("Dynamic Dragon Drive System/Input/ReinsInput Menu UI")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReinsInputMenuUI : UdonSharpBehaviour
    {
        public ReinsController reinsInput;

        [Space]
        [SerializeField] private GameObject _menu_thrust_normal;
        [SerializeField] private GameObject _menu_thrust_invert;
        //[SerializeField]
        private GameObject _menu_climb_normal;
        //[SerializeField]
        private GameObject _menu_climb_invert;
        //[SerializeField]
        private GameObject _menu_strafe_normal;
        //[SerializeField]
        private GameObject _menu_strafe_invert;
        [SerializeField] private GameObject _menu_elevator_normal;
        [SerializeField] private GameObject _menu_elevator_invert;
        [SerializeField] private GameObject _menu_ladder_normal;
        [SerializeField] private GameObject _menu_ladder_invert;
        [SerializeField] private GameObject _menu_aileron_normal;
        [SerializeField] private GameObject _menu_aileron_invert;
        [SerializeField] private GameObject _menu_throttleHand_right;
        [SerializeField] private GameObject _menu_throttleHand_left;
        [SerializeField] private GameObject _menu_turningHand_right;
        [SerializeField] private GameObject _menu_turningHand_left;
        [SerializeField] private GameObject _menu_elevatorHand_right;
        [SerializeField] private GameObject _menu_elevatorHand_left;

        private void OnEnable()
        {
            SetThrustSign(reinsInput.ThrustIsInvert);
            SetClimbSign(reinsInput.ClimbIsInvert);
            SetStrafeSign(reinsInput.StrafeIsInvert);

            SetElevatorSign(reinsInput.ElevatorIsInvert);
            SetLadderSign(reinsInput.LadderIsInvert);
            SetAileronSign(reinsInput.AileronIsInvert);

            SetThrottleHand(reinsInput.ThrottleInputHand);
            SetTurningHand(reinsInput.TurnInputHand);
            SetElevatorHand(reinsInput.ElevatorInputHand);
        }

        public void _InvertThrust()
        {
            reinsInput._InvertThrust();
            SetThrustSign(true);
        }
        public void _NormalThrust()
        {
            reinsInput._NormalThrust();
            SetThrustSign(false);
        }
        private void SetThrustSign(bool invert)
        {
            if (_menu_thrust_normal) { _menu_thrust_normal.SetActive(!invert); }
            if (_menu_thrust_invert) { _menu_thrust_invert.SetActive(invert); }
        }

        public void _InvertClimb()
        {
            reinsInput._InvertClimb();
            SetClimbSign(true);
        }
        public void _NormalClimb()
        {
            reinsInput._NormalClimb();
            SetClimbSign(false);

        }
        private void SetClimbSign(bool invert)
        {
            if (_menu_climb_normal) { _menu_climb_normal.SetActive(!invert); }
            if (_menu_climb_invert) { _menu_climb_invert.SetActive(invert); }
        }

        public void _InvertStrafe()
        {
            reinsInput._InvertStrafe();
            SetStrafeSign(true);
        }
        public void _NormalStrafe()
        {
            reinsInput._NormalStrafe();
            SetStrafeSign(false);

        }
        private void SetStrafeSign(bool invert)
        {
            if (_menu_strafe_normal) { _menu_strafe_normal.SetActive(!invert); }
            if (_menu_strafe_invert) { _menu_strafe_invert.SetActive(invert); }
        }

        public void _InvertElevator()
        {
            reinsInput._InvertElevator();
            SetElevatorSign(true);
        }
        public void _NormalElevator()
        {
            reinsInput._NormalElevator();
            SetElevatorSign(false);

        }
        private void SetElevatorSign(bool invert)
        {
            if (_menu_elevator_normal) { _menu_elevator_normal.SetActive(!invert); }
            if (_menu_elevator_invert) { _menu_elevator_invert.SetActive(invert); }
        }

        public void _InvertLadder()
        {
            reinsInput._InvertLadder();
            SetLadderSign(true);
        }
        public void _NormalLadder()
        {
            reinsInput._NormalLadder();
            SetLadderSign(false);
        }
        private void SetLadderSign(bool invert)
        {
            if (_menu_ladder_normal) { _menu_ladder_normal.SetActive(!invert); }
            if (_menu_ladder_invert) { _menu_ladder_invert.SetActive(invert); }
        }

        public void _InvertAileron()
        {
            reinsInput._InvertAileron();
            SetAileronSign(true);
        }
        public void _NormalAileron()
        {
            reinsInput._NormalAileron();
            SetAileronSign(false);
        }
        private void SetAileronSign(bool invert)
        {
            if (_menu_aileron_normal) { _menu_aileron_normal.SetActive(!invert); }
            if (_menu_aileron_invert) { _menu_aileron_invert.SetActive(invert); }
        }

        public void _SetThrottleRightHand()
        {
            reinsInput._SetThrottleRightHand();
            SetThrottleHand(HandType.RIGHT);
        }
        public void _SetThrottleLeftHand()
        {
            reinsInput._SetThrottleLeftHand();
            SetThrottleHand(HandType.LEFT);
        }
        private void SetThrottleHand(HandType hand)
        {
            if (_menu_throttleHand_right) { _menu_throttleHand_right.SetActive(hand == HandType.RIGHT); }
            if (_menu_throttleHand_left) { _menu_throttleHand_left.SetActive(hand == HandType.LEFT); }
        }

        public void _SetTurningRightHand()
        {
            reinsInput._SetTurningRightHand();
            SetTurningHand(HandType.RIGHT);
        }
        public void _SetTurningLeftHand()
        {
            reinsInput._SetTurningLeftHand();
            SetTurningHand(HandType.LEFT);
        }
        private void SetTurningHand(HandType hand)
        {
            if (_menu_turningHand_right) { _menu_turningHand_right.SetActive(hand == HandType.RIGHT); }
            if (_menu_turningHand_left) { _menu_turningHand_left.SetActive(hand == HandType.LEFT); }
        }

        public void _SetElevatorRightHand()
        {
            reinsInput._SetElevatorRightHand();
            SetElevatorHand(HandType.RIGHT);
        }
        public void _SetElevatorLeftHand()
        {
            reinsInput._SetElevatorLeftHand();
            SetElevatorHand(HandType.LEFT);
        }
        private void SetElevatorHand(HandType hand)
        {
            if (_menu_elevatorHand_right) { _menu_elevatorHand_right.SetActive(hand == HandType.RIGHT); }
            if (_menu_elevatorHand_left) { _menu_elevatorHand_left.SetActive(hand == HandType.LEFT); }
        }
    }
}
