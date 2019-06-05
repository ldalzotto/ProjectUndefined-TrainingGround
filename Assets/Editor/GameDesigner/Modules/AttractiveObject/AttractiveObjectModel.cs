using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AttractiveObjectModel : SetModelModule<AttractiveObjectType>
    {
        protected override Func<AttractiveObjectType, Transform> FindParent
        {
            get
            {
                return (AttractiveObjectType AttractiveObjectType) =>
                {
                    return AttractiveObjectType.transform;
                };
            }
        }
    }
}