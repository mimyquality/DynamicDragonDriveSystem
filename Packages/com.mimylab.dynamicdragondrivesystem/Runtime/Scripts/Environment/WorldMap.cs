/*
Copyright (c) 2024 Mimy Quality
Released under the MIT license
https://opensource.org/license/mit
*/

namespace MimyLab.DynamicDragonDriveSystem
{
    using UdonSharp;
    using UnityEngine;

    [Icon(ComponentIconPath.DDDSystem)]
    [AddComponentMenu("Dynamic Dragon Drive System/Environment/World Map")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WorldMap : UdonSharpBehaviour
    {
        [SerializeField]
        private WorldLocation[] _locations = new WorldLocation[0];

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

        public int GetLocation(Transform target)
        {
            Vector3 targetPoint = target.position;
            for (int i = 0; i < _locations.Length; i++)
            {
                if (_locations[i].CheckIsIn(targetPoint))
                {
                    return _locations[i].address;
                }
            }

            return 0;
        }
    }
}
