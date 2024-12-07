/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using System;
    using UdonSharp;
    using UnityEngine;
    //using VRC.SDKBase;
    //using VRC.Udon;
    //using VRC.SDK3.Components;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Interactions/Dragon Song")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class DragonSong : UdonSharpBehaviour
    {
        private const int MaxClipList = 10;

        [Tooltip("Execute the event name _SingHead plus the SFX element number in SendCustomEvent. For example, \"_SingHead1\".")]
        [SerializeField] private AudioSource _headSpeaker;
        [SerializeField] private AudioClip[] _headSFX = new AudioClip[0];
        [Tooltip("Execute the event name _SingBody plus the SFX element number in SendCustomEvent. For example, \"_SingBody1\".")]
        [SerializeField] private AudioSource _bodySpeaker;
        [SerializeField] private AudioClip[] _bodySFX = new AudioClip[0];
        [Tooltip("Execute the event name _SingFrontLeftLeg plus the SFX element number in SendCustomEvent. For example, \"_FrontLeftLeg1\".")]
        [SerializeField] private AudioSource _frontLeftLegSpeaker;
        [SerializeField] private AudioClip[] _frontLeftLegSFX = new AudioClip[0];
        [Tooltip("Execute the event name _SingFrontRightLeg plus the SFX element number in SendCustomEvent. For example, \"_SingFrontRightLeg1\".")]
        [SerializeField] private AudioSource _frontRightLegSpeaker;
        [SerializeField] private AudioClip[] _frontRightLegSFX = new AudioClip[0];
        [Tooltip("Execute the event name _SingRearLeftLeg plus the SFX element number in SendCustomEvent. For example, \"_SingRearLeftLeg1\".")]
        [SerializeField] private AudioSource _rearLeftLegSpeaker;
        [SerializeField] private AudioClip[] _rearLeftLegSFX = new AudioClip[0];
        [Tooltip("Execute the event name _SingRearRightLeg plus the SFX element number in SendCustomEvent. For example, \"_SingRearRightLeg1\".")]
        [SerializeField] private AudioSource _rearRightLegSpeaker;
        [SerializeField] private AudioClip[] _rearRightLegSFX = new AudioClip[0];
        [Tooltip("Execute the event name _SingLeftWing plus the SFX element number in SendCustomEvent. For example, \"_SingLeftWing1\".")]
        [SerializeField] private AudioSource _leftWingSpeaker;
        [SerializeField] private AudioClip[] _leftWingSFX = new AudioClip[0];
        [Tooltip("Execute the event name _SingRightWing plus the SFX element number in SendCustomEvent. For example, \"_SingRightWing1\".")]
        [SerializeField] private AudioSource _rightWingSpeaker;
        [SerializeField] private AudioClip[] _rightWingSFX = new AudioClip[0];
        [Tooltip("Execute the event name _SingTail plus the SFX element number in SendCustomEvent. For example, \"_SingTail1\".")]
        [SerializeField] private AudioSource _tailSpeaker;
        [SerializeField] private AudioClip[] _tailSFX = new AudioClip[0];

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        private void OnValidate()
        {
            LimitMaxClipLength(ref _headSFX);
            LimitMaxClipLength(ref _bodySFX);
            LimitMaxClipLength(ref _frontLeftLegSFX);
            LimitMaxClipLength(ref _frontRightLegSFX);
            LimitMaxClipLength(ref _rearLeftLegSFX);
            LimitMaxClipLength(ref _rearRightLegSFX);
            LimitMaxClipLength(ref _leftWingSFX);
            LimitMaxClipLength(ref _rightWingSFX);
            LimitMaxClipLength(ref _tailSFX);
        }
        private void LimitMaxClipLength(ref AudioClip[] clips)
        {
            if (clips.Length > MaxClipList)
            {
                Array.Resize(ref clips, MaxClipList);
            }
        }
#endif

        public void _SingHead0() { Sing(_headSpeaker, _headSFX, 0); }
        public void _SingHead1() { Sing(_headSpeaker, _headSFX, 1); }
        public void _SingHead2() { Sing(_headSpeaker, _headSFX, 2); }
        public void _SingHead3() { Sing(_headSpeaker, _headSFX, 3); }
        public void _SingHead4() { Sing(_headSpeaker, _headSFX, 4); }
        public void _SingHead5() { Sing(_headSpeaker, _headSFX, 5); }
        public void _SingHead6() { Sing(_headSpeaker, _headSFX, 6); }
        public void _SingHead7() { Sing(_headSpeaker, _headSFX, 7); }
        public void _SingHead8() { Sing(_headSpeaker, _headSFX, 8); }
        public void _SingHead9() { Sing(_headSpeaker, _headSFX, 9); }

        public void _SingBody0() { Sing(_bodySpeaker, _bodySFX, 0); }
        public void _SingBody1() { Sing(_bodySpeaker, _bodySFX, 1); }
        public void _SingBody2() { Sing(_bodySpeaker, _bodySFX, 2); }
        public void _SingBody3() { Sing(_bodySpeaker, _bodySFX, 3); }
        public void _SingBody4() { Sing(_bodySpeaker, _bodySFX, 4); }
        public void _SingBody5() { Sing(_bodySpeaker, _bodySFX, 5); }
        public void _SingBody6() { Sing(_bodySpeaker, _bodySFX, 6); }
        public void _SingBody7() { Sing(_bodySpeaker, _bodySFX, 7); }
        public void _SingBody8() { Sing(_bodySpeaker, _bodySFX, 8); }
        public void _SingBody9() { Sing(_bodySpeaker, _bodySFX, 9); }

        public void _SingFrontLeftLeg0() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 0); }
        public void _SingFrontLeftLeg1() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 1); }
        public void _SingFrontLeftLeg2() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 2); }
        public void _SingFrontLeftLeg3() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 3); }
        public void _SingFrontLeftLeg4() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 4); }
        public void _SingFrontLeftLeg5() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 5); }
        public void _SingFrontLeftLeg6() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 6); }
        public void _SingFrontLeftLeg7() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 7); }
        public void _SingFrontLeftLeg8() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 8); }
        public void _SingFrontLeftLeg9() { Sing(_frontLeftLegSpeaker, _frontLeftLegSFX, 9); }

        public void _SingFrontRightLeg0() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 0); }
        public void _SingFrontRightLeg1() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 1); }
        public void _SingFrontRightLeg2() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 2); }
        public void _SingFrontRightLeg3() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 3); }
        public void _SingFrontRightLeg4() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 4); }
        public void _SingFrontRightLeg5() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 5); }
        public void _SingFrontRightLeg6() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 6); }
        public void _SingFrontRightLeg7() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 7); }
        public void _SingFrontRightLeg8() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 8); }
        public void _SingFrontRightLeg9() { Sing(_frontRightLegSpeaker, _frontRightLegSFX, 9); }

        public void _SingRearLeftLeg0() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 0); }
        public void _SingRearLeftLeg1() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 1); }
        public void _SingRearLeftLeg2() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 2); }
        public void _SingRearLeftLeg3() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 3); }
        public void _SingRearLeftLeg4() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 4); }
        public void _SingRearLeftLeg5() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 5); }
        public void _SingRearLeftLeg6() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 6); }
        public void _SingRearLeftLeg7() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 7); }
        public void _SingRearLeftLeg8() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 8); }
        public void _SingRearLeftLeg9() { Sing(_rearLeftLegSpeaker, _rearLeftLegSFX, 9); }

        public void _SingRearRightLeg0() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 0); }
        public void _SingRearRightLeg1() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 1); }
        public void _SingRearRightLeg2() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 2); }
        public void _SingRearRightLeg3() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 3); }
        public void _SingRearRightLeg4() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 4); }
        public void _SingRearRightLeg5() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 5); }
        public void _SingRearRightLeg6() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 6); }
        public void _SingRearRightLeg7() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 7); }
        public void _SingRearRightLeg8() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 8); }
        public void _SingRearRightLeg9() { Sing(_rearRightLegSpeaker, _rearRightLegSFX, 9); }

        public void _SingLeftWing0() { Sing(_leftWingSpeaker, _leftWingSFX, 0); }
        public void _SingLeftWing1() { Sing(_leftWingSpeaker, _leftWingSFX, 1); }
        public void _SingLeftWing2() { Sing(_leftWingSpeaker, _leftWingSFX, 2); }
        public void _SingLeftWing3() { Sing(_leftWingSpeaker, _leftWingSFX, 3); }
        public void _SingLeftWing4() { Sing(_leftWingSpeaker, _leftWingSFX, 4); }
        public void _SingLeftWing5() { Sing(_leftWingSpeaker, _leftWingSFX, 5); }
        public void _SingLeftWing6() { Sing(_leftWingSpeaker, _leftWingSFX, 6); }
        public void _SingLeftWing7() { Sing(_leftWingSpeaker, _leftWingSFX, 7); }
        public void _SingLeftWing8() { Sing(_leftWingSpeaker, _leftWingSFX, 8); }
        public void _SingLeftWing9() { Sing(_leftWingSpeaker, _leftWingSFX, 9); }

        public void _SingRightWing0() { Sing(_rightWingSpeaker, _rightWingSFX, 0); }
        public void _SingRightWing1() { Sing(_rightWingSpeaker, _rightWingSFX, 1); }
        public void _SingRightWing2() { Sing(_rightWingSpeaker, _rightWingSFX, 2); }
        public void _SingRightWing3() { Sing(_rightWingSpeaker, _rightWingSFX, 3); }
        public void _SingRightWing4() { Sing(_rightWingSpeaker, _rightWingSFX, 4); }
        public void _SingRightWing5() { Sing(_rightWingSpeaker, _rightWingSFX, 5); }
        public void _SingRightWing6() { Sing(_rightWingSpeaker, _rightWingSFX, 6); }
        public void _SingRightWing7() { Sing(_rightWingSpeaker, _rightWingSFX, 7); }
        public void _SingRightWing8() { Sing(_rightWingSpeaker, _rightWingSFX, 8); }
        public void _SingRightWing9() { Sing(_rightWingSpeaker, _rightWingSFX, 9); }

        public void _SingTail0() { Sing(_tailSpeaker, _tailSFX, 0); }
        public void _SingTail1() { Sing(_tailSpeaker, _tailSFX, 1); }
        public void _SingTail2() { Sing(_tailSpeaker, _tailSFX, 2); }
        public void _SingTail3() { Sing(_tailSpeaker, _tailSFX, 3); }
        public void _SingTail4() { Sing(_tailSpeaker, _tailSFX, 4); }
        public void _SingTail5() { Sing(_tailSpeaker, _tailSFX, 5); }
        public void _SingTail6() { Sing(_tailSpeaker, _tailSFX, 6); }
        public void _SingTail7() { Sing(_tailSpeaker, _tailSFX, 7); }
        public void _SingTail8() { Sing(_tailSpeaker, _tailSFX, 8); }
        public void _SingTail9() { Sing(_tailSpeaker, _tailSFX, 9); }

        private void Sing(AudioSource target, AudioClip[] sound, int index)
        {
            if (!target) { return; }
            if (index >= sound.Length) { return; }
            if (!sound[index]) { return; }

            target.PlayOneShot(sound[index], target.volume);
        }
    }
}
