﻿/*
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
    //using VRC.SDK3.Components;

    [AddComponentMenu("Dynamic Dragon Drive System/Misc/ReactiveTrigger to Dragon")]
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReactiveTriggerToDragon : UdonSharpBehaviour
    {
        [SerializeField, Header("Activate/Inactivate GameObject")]
        private GameObject _ActivateObject;
        [SerializeField]
        private GameObject _InactivateObject;
        [SerializeField, Tooltip("Reactivate after specified time if Duration > 0")]
        private float _duration = 0.0f;

        [Header("Emit particle")]
        [SerializeField]
        private ParticleSystem _particleSystem;
        [SerializeField]
        private int _emit = 0;

        [Header("Play sound one shot")]
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _audioClip;

        [Header("Execute SendCustomEvent to other UdonBehaviour")]
        [SerializeField,]
        private UdonBehaviour _udonBehaviour;
        [SerializeField]
        private string _eventName = "";

        [Header("Other Options")]
        [SerializeField]
        private bool _dragonOwnerOnly = false;

        private int _triggerCount = 0;

        private void Start()
        {
            if (!_particleSystem) { _particleSystem = GetComponent<ParticleSystem>(); }
            if (!_audioSource) { _audioSource = GetComponent<AudioSource>(); }
            if (_audioSource) { if (!_audioClip) { _audioClip = _audioSource.clip; } }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other) { return; }
            var rb = other.attachedRigidbody;
            if (!rb) { return; }
            var dragon = rb.GetComponent<DragonDriver>();
            if (!dragon) { return; }

            if (_dragonOwnerOnly && !Networking.IsOwner(dragon.gameObject)) { return; }

            if (_ActivateObject || _InactivateObject)
            {
                ++_triggerCount;

                if (_ActivateObject) { _ActivateObject.SetActive(true); }
                if (_InactivateObject) { _InactivateObject.SetActive(false); }

                if (_duration > 0.0f) { SendCustomEventDelayedSeconds(nameof(_ReactivateGameObject), _duration); }
            }
            if (_particleSystem && _emit > 0) { _particleSystem.Emit(_emit); }
            if (_audioSource && _audioClip) { _audioSource.PlayOneShot(_audioClip, _audioSource.volume); }
            if (_udonBehaviour && _eventName != "") { _udonBehaviour.SendCustomEvent(_eventName); }
        }

        public void _ReactivateGameObject()
        {
            if (--_triggerCount > 0) { return; }

            if (_ActivateObject) { _ActivateObject.SetActive(false); }
            if (_InactivateObject) { _InactivateObject.SetActive(true); }
        }
    }
}
