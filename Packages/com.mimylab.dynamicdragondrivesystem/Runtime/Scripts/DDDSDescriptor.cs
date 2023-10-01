
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

namespace MimyLab.DynamicDragonDriveSystem
{
    [DefaultExecutionOrder(-1000)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DDDSDescriptor : UdonSharpBehaviour
    {
        public DragonDriver driver;
        public DragonSaddle saddle;
        public DragonActor actor;
        public DragonReinsSelecter reinsSelecter;
        public SummonDragonSwitch summonSwitch;

        private bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) { return; }

            driver = GetComponentInChildren<DragonDriver>();
            saddle = GetComponentInChildren<DragonSaddle>();
            actor = GetComponentInChildren<DragonActor>();
            reinsSelecter = GetComponentInChildren<DragonReinsSelecter>();
            summonSwitch = GetComponentInChildren<SummonDragonSwitch>();

            saddle.driver = driver;
            saddle.actor = actor;
            saddle.summonSwitch = summonSwitch;
            actor.driver = driver;
            reinsSelecter.driver = driver;
            reinsSelecter.saddle = saddle;
            summonSwitch.driver = driver;

            _initialized = true;
        }
        private void Start()
        {
            Initialize();
        }
    }
}
