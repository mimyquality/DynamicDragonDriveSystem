/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using Random = UnityEngine.Random;

namespace MimyLab.DynamicDragonDriveSystem
{
    public enum DragonActorParameterName
    {
        // Triggerパラメーター
        OnBlink, OnBlinkMembrane,
        OnCollision,
        // Boolパラメーター
        IsOwner, IsLocal,
        IsPilot,
        IsRide, IsMount,
        IsGrounded,
        IsBrakes,
        IsOverdrive,
        // Intパラメーター
        State,
        Location,
        // Floatパラメーター
        NosePitch,
        NoseYaw,
        Pitch,
        Roll,
        VelocityX, VelocityY, VelocityZ,
        VelocityMagnitude, Speed,
        AngularVelocityX, AngularVelocityY, AngularVelocityZ,
        AngularVelocityMagnitude, AngularSpeed,
        Throttle,
        Turn,
        Elevator,
    }

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Core/Dragon Actor")]
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(10)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DragonActor : UdonSharpBehaviour
    {
        private const float AngleSyncTolerance = 0.1f; // 単位：°
        private const float SmoothingDuration = 0.05f;   // 単位：sec
        private const float GroundedSmoothingDuration = 0.08f;   // 単位：sec

        [SerializeField, Min(0.0f), Tooltip("sec")]
        private float _blinkRateMin = 20.0f;
        [SerializeField, Min(0.0f), Tooltip("sec")]
        private float _blinkRateMax = 60.0f;
        [SerializeField, Min(0.0f), Tooltip("sec")]
        private float _blinkMembraneRateMin = 4.0f;
        [SerializeField, Min(0.0f), Tooltip("sec")]
        private float _blinkMembraneRateMax = 20.0f;

        [Header("Advanced Options")]
        [SerializeField, Tooltip("Required if you want to use the Location parameter.")]
        private WorldMap _worldMap;

        [UdonSynced]
        private bool n_isBrakes;
        [UdonSynced]
        private bool n_isOverdrive;
        [UdonSynced]
        private int n_state;
        [UdonSynced]
        private Vector3 n_noseAngles;

        internal DragonDriver _driver;
        internal DragonReins _reins;
        internal DragonRider _rider;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private VRCVelocityTracker _velocityTracker;
        private Quaternion _rotation;

        private bool _isOwner;
        private bool _isPilot;
        private bool _isRide, _isMount;
        private bool _isGrounded;
        private Vector3 _noseAngles;
        private Vector3 _angleVelocity;
        private float _groundedNoseAngle;
        private float _groundedAngleVelocity;
        private float _pitch, _roll;
        private Vector3 _relativeVelocity;
        private Vector3 _relativeAngularVelocity;
        private float _speed, _angularSpeed;
        private float _throttle, _turn, _elevator;
        private int _location;

        private int[] _parameterHashes =
        {
            // Triggerパラメーター
            Animator.StringToHash(nameof(DragonActorParameterName.OnBlink)),
            Animator.StringToHash(nameof(DragonActorParameterName.OnBlinkMembrane)),
            Animator.StringToHash(nameof(DragonActorParameterName.OnCollision)),
            // Boolパラメーター
            Animator.StringToHash(nameof(DragonActorParameterName.IsOwner)),
            Animator.StringToHash(nameof(DragonActorParameterName.IsLocal)),
            Animator.StringToHash(nameof(DragonActorParameterName.IsPilot)),
            Animator.StringToHash(nameof(DragonActorParameterName.IsRide)),
            Animator.StringToHash(nameof(DragonActorParameterName.IsMount)),
            Animator.StringToHash(nameof(DragonActorParameterName.IsGrounded)),
            Animator.StringToHash(nameof(DragonActorParameterName.IsBrakes)),
            Animator.StringToHash(nameof(DragonActorParameterName.IsOverdrive)),
            // Intパラメーター
            Animator.StringToHash(nameof(DragonActorParameterName.State)),
            Animator.StringToHash(nameof(DragonActorParameterName.Location)),
            // Floatパラメーター
            Animator.StringToHash(nameof(DragonActorParameterName.NosePitch)),
            Animator.StringToHash(nameof(DragonActorParameterName.NoseYaw)),
            Animator.StringToHash(nameof(DragonActorParameterName.Pitch)),
            Animator.StringToHash(nameof(DragonActorParameterName.Roll)),
            Animator.StringToHash(nameof(DragonActorParameterName.VelocityX)),
            Animator.StringToHash(nameof(DragonActorParameterName.VelocityY)),
            Animator.StringToHash(nameof(DragonActorParameterName.VelocityZ)),
            Animator.StringToHash(nameof(DragonActorParameterName.VelocityMagnitude)),
            Animator.StringToHash(nameof(DragonActorParameterName.Speed)),
            Animator.StringToHash(nameof(DragonActorParameterName.AngularVelocityX)),
            Animator.StringToHash(nameof(DragonActorParameterName.AngularVelocityY)),
            Animator.StringToHash(nameof(DragonActorParameterName.AngularVelocityZ)),
            Animator.StringToHash(nameof(DragonActorParameterName.AngularVelocityMagnitude)),
            Animator.StringToHash(nameof(DragonActorParameterName.AngularSpeed)),
            Animator.StringToHash(nameof(DragonActorParameterName.Throttle)),
            Animator.StringToHash(nameof(DragonActorParameterName.Turn)),
            Animator.StringToHash(nameof(DragonActorParameterName.Elevator))
        };
        private bool[] _validParameters;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) return;

            _animator = GetComponent<Animator>();
            _rigidbody = _driver.GetComponent<Rigidbody>();
            _velocityTracker = _driver.GetComponent<VRCVelocityTracker>();

            _animator.applyRootMotion = false;
            _validParameters = ValidateParameters(_animator, _parameterHashes);

            _initialized = true;
        }
        private void Start()
        {
            Initialize();

            _TriggerBlink();
            _TriggerBlinkMembrane();
        }

        private void Update()
        {
            _rotation = _rigidbody.rotation;
            _isOwner = Networking.IsOwner(_driver.gameObject);
            UpdateSyncParameters();
            CalculateParameters();
            SetAnimatorParameters();
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        // Animator.applyRootMotionを強制的にオフにする
        private void OnAnimatorMove() { }
#endif

        public void _TriggerBlink()
        {
            if (!_validParameters[(int)DragonActorParameterName.OnBlink]) return;

            _animator.SetTrigger(_parameterHashes[(int)DragonActorParameterName.OnBlink]);

            float nextTiming = Random.Range(_blinkRateMin, _blinkRateMax);
            SendCustomEventDelayedSeconds(nameof(_TriggerBlink), nextTiming);
        }

        public void _TriggerBlinkMembrane()
        {
            if (!_validParameters[(int)DragonActorParameterName.OnBlinkMembrane]) return;

            _animator.SetTrigger(_parameterHashes[(int)DragonActorParameterName.OnBlinkMembrane]);

            float nextTiming = Random.Range(_blinkMembraneRateMin, _blinkMembraneRateMax);
            SendCustomEventDelayedSeconds(nameof(_TriggerBlinkMembrane), nextTiming);
        }

        public void _TriggerCollision()
        {
            if (!_initialized) return;
            if (!_validParameters[(int)DragonActorParameterName.OnCollision]) return;

            _animator.SetTrigger(_parameterHashes[(int)DragonActorParameterName.OnCollision]);
        }

        private static bool[] ValidateParameters(Animator animator, int[] parametersHash)
        {
            var result = new bool[parametersHash.Length];

            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                int validIndex = Array.IndexOf(parametersHash, parameters[i].nameHash);
                if (validIndex > -1) { result[validIndex] = true; }
            }

            return result;
        }

        private void UpdateSyncParameters()
        {
            if (!Networking.IsOwner(gameObject))
            {
                _noseAngles.x = Mathf.SmoothDamp(_noseAngles.x, n_noseAngles.x, ref _angleVelocity.x, SmoothingDuration);
                _noseAngles.y = Mathf.SmoothDamp(_noseAngles.y, n_noseAngles.y, ref _angleVelocity.y, SmoothingDuration);

                return;
            }

            if (n_isBrakes != _driver.IsBrakes)
            {
                n_isBrakes = !n_isBrakes;
                RequestSerialization();
            }
            if (n_isOverdrive != _driver.IsOverdrive)
            {
                n_isOverdrive = !n_isOverdrive;
                RequestSerialization();
            }

            if (n_state != _driver.State)
            {
                n_state = _driver.State;
                RequestSerialization();
            }

            Vector3 noseAngles = _driver.NoseAngles;
            // Animator向けに反転
            _noseAngles.x = (n_state == (int)DragonDriverStateType.Walking) ? 0.0f : -noseAngles.x;
            _noseAngles.y = noseAngles.y;
            if (!((Mathf.Abs(n_noseAngles.x - _noseAngles.x) < AngleSyncTolerance)
               && (Mathf.Abs(n_noseAngles.y - _noseAngles.y) < AngleSyncTolerance)))
            {
                n_noseAngles = _noseAngles;
                RequestSerialization();
            }
        }

        private void CalculateParameters()
        {
            _isPilot = _rider.IsPilot;
            _isRide = _rider.IsRide;
            _isMount = _rider.IsMount;

            _isGrounded = _driver.IsGrounded;

            if (n_state == (int)DragonDriverStateType.Walking)
            {
                var pitch = 0.0f;
                if (_isGrounded)
                {
                    Vector3 groundNormal = _driver.GroundNormal;
                    Quaternion noseRotation = Quaternion.Euler(pitch, _noseAngles.y, 0.0f) * _rotation;
                    Vector3 noseDirection = noseRotation * Vector3.forward;
                    Vector3 noseLeft = noseRotation * Vector3.left;
                    Vector3 horizontalLeft = Vector3.ProjectOnPlane(noseLeft, groundNormal);
                    float tiltCorrection = Mathf.Abs(Vector3.Dot(noseLeft, horizontalLeft));
                    Vector3 groundForward = Vector3.ProjectOnPlane(noseDirection, groundNormal);
                    pitch = tiltCorrection * Vector3.SignedAngle(noseDirection, groundForward, noseLeft);
                }
                _groundedNoseAngle = Mathf.SmoothDamp(_groundedNoseAngle, pitch, ref _groundedAngleVelocity, GroundedSmoothingDuration);
                _noseAngles.x = _groundedNoseAngle;
            }
            else
            {
                _groundedNoseAngle = _noseAngles.x;
            }

            Vector3 forward = _rotation * Vector3.forward;
            Vector3 up = _rotation * Vector3.up;

            Vector3 level = Vector3.ProjectOnPlane(forward, Vector3.up);
            Vector3 left = Quaternion.LookRotation(level) * Vector3.left;
            _pitch = Vector3.SignedAngle(level, forward, left);

            level = Quaternion.LookRotation(forward) * Vector3.up;
            _roll = Vector3.SignedAngle(level, up, forward);

            _relativeVelocity = Quaternion.Inverse(_rotation) * _velocityTracker.GetLinearVelocity();
            _relativeAngularVelocity = Quaternion.Inverse(_rotation) * _velocityTracker.GetAngularVelocity();

            _speed = _relativeVelocity.magnitude;
            _angularSpeed = _relativeAngularVelocity.magnitude;

            ReinsInputManager reinsInput = _reins._GetEnabledInput();
            if (reinsInput)
            {
                _throttle = reinsInput.Thrust;
                _turn = reinsInput.Turn;
                _elevator = reinsInput.Elevator;
            }
            else
            {
                _throttle = 0.0f;
                _turn = 0.0f;
                _elevator = 0.0f;
            }

            if (_worldMap)
            {
                _location = _worldMap.GetLocation(_driver.transform);
            }
        }

        private void SetAnimatorParameters()
        {
            if (_validParameters[(int)DragonActorParameterName.IsOwner]) { _animator.SetBool(_parameterHashes[(int)DragonActorParameterName.IsOwner], _isOwner); }
            if (_validParameters[(int)DragonActorParameterName.IsLocal]) _animator.SetBool(_parameterHashes[(int)DragonActorParameterName.IsLocal], _isOwner);
            if (_validParameters[(int)DragonActorParameterName.IsPilot]) _animator.SetBool(_parameterHashes[(int)DragonActorParameterName.IsPilot], _isPilot);
            if (_validParameters[(int)DragonActorParameterName.IsRide]) _animator.SetBool(_parameterHashes[(int)DragonActorParameterName.IsRide], _isRide);
            if (_validParameters[(int)DragonActorParameterName.IsMount]) _animator.SetBool(_parameterHashes[(int)DragonActorParameterName.IsMount], _isMount);
            if (_validParameters[(int)DragonActorParameterName.IsGrounded]) _animator.SetBool(_parameterHashes[(int)DragonActorParameterName.IsGrounded], _isGrounded);
            if (_validParameters[(int)DragonActorParameterName.IsBrakes]) _animator.SetBool(_parameterHashes[(int)DragonActorParameterName.IsBrakes], n_isBrakes);
            if (_validParameters[(int)DragonActorParameterName.IsOverdrive]) _animator.SetBool(_parameterHashes[(int)DragonActorParameterName.IsOverdrive], n_isOverdrive);

            if (_validParameters[(int)DragonActorParameterName.State]) _animator.SetInteger(_parameterHashes[(int)DragonActorParameterName.State], n_state);
            if (_validParameters[(int)DragonActorParameterName.Location]) _animator.SetInteger(_parameterHashes[(int)DragonActorParameterName.Location], _location);

            if (_validParameters[(int)DragonActorParameterName.NosePitch]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.NosePitch], _noseAngles.x);
            if (_validParameters[(int)DragonActorParameterName.NoseYaw]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.NoseYaw], _noseAngles.y);
            if (_validParameters[(int)DragonActorParameterName.Pitch]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.Pitch], _pitch);
            if (_validParameters[(int)DragonActorParameterName.Roll]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.Roll], _roll);
            if (_validParameters[(int)DragonActorParameterName.VelocityX]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.VelocityX], _relativeVelocity.x);
            if (_validParameters[(int)DragonActorParameterName.VelocityY]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.VelocityY], _relativeVelocity.y);
            if (_validParameters[(int)DragonActorParameterName.VelocityZ]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.VelocityZ], _relativeVelocity.z);
            if (_validParameters[(int)DragonActorParameterName.VelocityMagnitude]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.VelocityMagnitude], _speed);
            if (_validParameters[(int)DragonActorParameterName.Speed]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.Speed], _speed);
            if (_validParameters[(int)DragonActorParameterName.AngularVelocityX]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.AngularVelocityX], _relativeAngularVelocity.x);
            if (_validParameters[(int)DragonActorParameterName.AngularVelocityY]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.AngularVelocityY], _relativeAngularVelocity.y);
            if (_validParameters[(int)DragonActorParameterName.AngularVelocityZ]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.AngularVelocityZ], _relativeAngularVelocity.z);
            if (_validParameters[(int)DragonActorParameterName.AngularVelocityMagnitude]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.AngularVelocityMagnitude], _angularSpeed);
            if (_validParameters[(int)DragonActorParameterName.AngularSpeed]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.AngularSpeed], _angularSpeed);
            if (_validParameters[(int)DragonActorParameterName.Throttle]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.Throttle], _throttle);
            if (_validParameters[(int)DragonActorParameterName.Turn]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.Turn], _turn);
            if (_validParameters[(int)DragonActorParameterName.Elevator]) _animator.SetFloat(_parameterHashes[(int)DragonActorParameterName.Elevator], _elevator);
        }
    }
}
