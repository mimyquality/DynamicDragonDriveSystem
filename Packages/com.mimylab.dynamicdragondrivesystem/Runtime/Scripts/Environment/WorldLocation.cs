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
    [AddComponentMenu("Dynamic Dragon Drive System/Environment/World Location")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WorldLocation : UdonSharpBehaviour
    {
        public int address = 1;

        [SerializeField]
        private Collider[] _area = new Collider[0];

        public bool CheckIsIn(Vector3 target)
        {
            var isIn = false;

            foreach (Collider col in _area)
            {
                if (!col) { continue; }

                if (target == col.ClosestPoint(target))
                {
                    isIn = true;
                    break;
                }
            }

            return isIn;
        }
    }
}
