/*
Copyright (c) 2023 Mimy Quality
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

    [UdonBehaviourSyncMode(BehaviourSyncMode.Any)]
    public class DragonEffecter : UdonSharpBehaviour
    {
        [SerializeField] AudioSource _mouth;
        [SerializeField] AudioSource _body;
        [SerializeField] AudioSource _rightHand;
        [SerializeField] AudioSource _leftHand;
        [SerializeField] AudioSource _rightLeg;
        [SerializeField] AudioSource _leftLeg;
        [SerializeField] AudioSource _rightWing;
        [SerializeField] AudioSource _leftWing;
        [SerializeField] AudioSource _tail;

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
