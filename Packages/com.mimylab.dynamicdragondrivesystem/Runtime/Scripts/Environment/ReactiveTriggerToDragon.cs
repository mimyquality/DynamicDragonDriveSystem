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
    using VRC.Udon;

    public enum ReactiveTriggerEventTarget
    {
        All,
        Owner,
        Pilot,
        Rider,
    }

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Environment/ReactiveTrigger to Dragon")]
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReactiveTriggerToDragon : UdonSharpBehaviour
    {
        [SerializeField]
        private ReactiveTriggerEventTarget _eventTarget = ReactiveTriggerEventTarget.All;

        [SerializeField, Header("Toggle GameObjects Active")]
        private GameObject[] _gameObjects = new GameObject[0];
        [SerializeField]
        private bool _invert = false;
        [SerializeField, Min(0), Tooltip("Re-toggle after specified time if Duration > 0")]
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

            Rigidbody rb = other.attachedRigidbody;
            if (!rb) { return; }

            var dragon = rb.GetComponent<DragonDriver>();
            if (!dragon) { return; }
            if (!ValidateEventTarget(dragon)) { return; }

            if (_gameObjects.Length > 0)
            {
                ++_triggerCount;

                for (int i = 0; i < _gameObjects.Length; i++)
                {
                    if (_gameObjects[i]) { _gameObjects[i].SetActive(!_invert); }
                }

                if (_duration > 0.0f) { SendCustomEventDelayedSeconds(nameof(_ToggleActiveDelayed), _duration); }
            }
            if (_particleSystem && _emit > 0) { _particleSystem.Emit(_emit); }
            if (_audioSource && _audioClip) { _audioSource.PlayOneShot(_audioClip); }
            if (_udonBehaviour && _eventName != "") { _udonBehaviour.SendCustomEvent(_eventName); }
        }

        private bool ValidateEventTarget(DragonDriver driver)
        {
            DragonRider rider = driver._rider;
            switch (_eventTarget)
            {
                case ReactiveTriggerEventTarget.All:
                    return true;
                case ReactiveTriggerEventTarget.Owner:
                    if (Networking.IsOwner(this.gameObject)) { return true; }
                    break;
                case ReactiveTriggerEventTarget.Pilot:
                    if (rider.IsPilot) { return true; }
                    break;
                case ReactiveTriggerEventTarget.Rider:
                    if (rider.IsRide) { return true; }
                    break;
            }

            return false;
        }

        public void _ToggleActiveDelayed()
        {
            if (--_triggerCount > 0) { return; }

            for (int i = 0; i < _gameObjects.Length; i++)
            {
                if (_gameObjects[i]) { _gameObjects[i].SetActive(_invert); }
            }
        }
    }
}
