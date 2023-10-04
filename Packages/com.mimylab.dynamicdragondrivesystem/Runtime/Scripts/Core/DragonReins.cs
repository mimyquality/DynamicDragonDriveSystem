/*
Copyright (c) 2023 Mimy Quality
Released under the MIT license
https://opensource.org/licenses/mit-license.php
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.Udon.Common;

namespace MimyLab.DynamicDragonDriveSystem
{
    [DefaultExecutionOrder(-200)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DragonReins : UdonSharpBehaviour
    {
        public DragonDriver driver;
        public DragonSaddle saddle;

        public ReinsInputKB keyboard;
        public ReinsInputSTK thumbsticks;
        public ReinsInputVR vrHands;
        //public ReinsInputTP touchPad;


        [SerializeField] private GameObject selectedKeyboard;
        [SerializeField] private GameObject selectedThumbsticks;
        [SerializeField] private GameObject selectedVRHands;
        //[SerializeField] private GameObject selectedTouchPad;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            keyboard = GetComponentInChildren<ReinsInputKB>(true);
            thumbsticks = GetComponentInChildren<ReinsInputSTK>(true);
            vrHands = GetComponentInChildren<ReinsInputVR>(true);
            //touchPad = GetComponentInChildren<ReinsInputTP>();

            keyboard.driver = driver;
            keyboard.saddle = saddle;
            keyboard.enabled = false;

            thumbsticks.driver = driver;
            thumbsticks.saddle = saddle;
            thumbsticks.enabled = false;

            vrHands.driver = driver;
            vrHands.saddle = saddle;
            vrHands.enabled = false;

            // touchPad初期化

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            if (Networking.LocalPlayer.IsUserInVR())
            {
                SetThumbsticks();
            }
            else
            {
                SetKeyboard();
            }
        }

        public void SetKeyboard()
        {
            saddle.reins = keyboard;

            selectedKeyboard.SetActive(true);
            selectedThumbsticks.SetActive(false);
            selectedVRHands.SetActive(false);
        }

        public void SetThumbsticks()
        {
            saddle.reins = thumbsticks;

            selectedKeyboard.SetActive(false);
            selectedThumbsticks.SetActive(true);
            selectedVRHands.SetActive(false);
        }

        public void SetVRHands()
        {
            saddle.reins = vrHands;

            selectedKeyboard.SetActive(false);
            selectedThumbsticks.SetActive(false);
            selectedVRHands.SetActive(true);
        }

        public void SetTouchPad() { }
    }
}
