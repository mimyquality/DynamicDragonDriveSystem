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

    [AddComponentMenu("Dynamic Dragon Drive System/Dragon Effector")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Any)]
    public class DragonEffector : UdonSharpBehaviour
    {
        [SerializeField]
        private AudioSource _mouth;
        [SerializeField]
        private AudioSource _body;
        [SerializeField]
        private AudioSource _rightHand;
        [SerializeField]
        private AudioSource _leftHand;
        [SerializeField]
        private AudioSource _rightLeg;
        [SerializeField]
        private AudioSource _leftLeg;
        [SerializeField]
        private AudioSource _rightWing;
        [SerializeField]
        private AudioSource _leftWing;
        [SerializeField]
        private AudioSource _tail;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }



            _initialized = true;
        }
        private void Start()
        {
            Initialize();


        }
    }
}
