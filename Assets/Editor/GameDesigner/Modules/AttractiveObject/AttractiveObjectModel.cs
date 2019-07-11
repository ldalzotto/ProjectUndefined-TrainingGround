using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AttractiveObjectModel : SetModelModule<AttractiveObjectTypeModule>
    {
        protected override Func<AttractiveObjectTypeModule, Transform> FindParent
        {
            get
            {
                return (AttractiveObjectTypeModule AttractiveObjectType) =>
                {
                    return AttractiveObjectType.transform;
                };
            }
        }
    }
}