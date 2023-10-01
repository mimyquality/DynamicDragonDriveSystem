
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
    public class DragonReinsSelecter : UdonSharpBehaviour
    {
        public DragonDriver driver;
        public DragonSaddle saddle;

        public KBDragonReins keyboard;
        public STKDragonReins joysticks;
        public VRDragonReins vrHand;
        //public PADDragonReins touchPad;

        [SerializeField] private GameObject selectedKeyboard;
        [SerializeField] private GameObject selectedJoysticks;
        [SerializeField] private GameObject selectedVRHands;
        [SerializeField] private GameObject selectedTouchPad;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            keyboard = GetComponentInChildren<KBDragonReins>();
            joysticks = GetComponentInChildren<STKDragonReins>();
            vrHand = GetComponentInChildren<VRDragonReins>();
            //touchPad = GetComponentInChildren<PADDragonReins>();

            keyboard.driver = driver;
            keyboard.saddle = saddle;
            keyboard.enabled = false;

            joysticks.driver = driver;
            joysticks.saddle = saddle;
            joysticks.enabled = false;

            vrHand.driver = driver;
            vrHand.saddle = saddle;
            vrHand.enabled = false;

            // touchPad初期化

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            if (Networking.LocalPlayer.IsUserInVR())
            {
                SetJoysticks();
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
            selectedJoysticks.SetActive(false);
            selectedVRHands.SetActive(false);
            //selectedTouchPad.SetActive(false);
        }

        public void SetJoysticks()
        {
            saddle.reins = joysticks;

            selectedKeyboard.SetActive(false);
            selectedJoysticks.SetActive(true);
            selectedVRHands.SetActive(false);
            //selectedTouchPad.SetActive(false);
        }

        public void SetVRHands()
        {
            saddle.reins = vrHand;

            selectedKeyboard.SetActive(false);
            selectedJoysticks.SetActive(false);
            selectedVRHands.SetActive(true);
            //selectedTouchPad.SetActive(false);
        }

        public void SetTouchPad()
        {
            //saddle.reins = touchPad;

            selectedKeyboard.SetActive(false);
            selectedJoysticks.SetActive(false);
            selectedVRHands.SetActive(false);
            selectedTouchPad.SetActive(true);
        }
    }
}
