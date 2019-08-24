using RTPuzzle;
using System;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AIModel : SetModelModule<AIObjectType>
    {
        public GameObject AIModelObject;

        protected override Func<AIObjectType, Transform> FindParent
        {
            get
            {
                return (AIObjectType NPCAIManager) =>
                {
                    return NPCAIManager.gameObject.FindChildObjectRecursively("Model").transform;
                };
            }
        }
    }
}