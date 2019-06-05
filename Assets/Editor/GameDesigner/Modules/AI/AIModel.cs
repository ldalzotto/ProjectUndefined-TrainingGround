using RTPuzzle;
using System;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class AIModel : SetModelModule<NPCAIManager>
    {
        public GameObject AIModelObject;

        protected override Func<NPCAIManager, Transform> FindParent
        {
            get
            {
                return (NPCAIManager NPCAIManager) =>
                {
                    return NPCAIManager.gameObject.FindChildObjectRecursively("Model").transform;
                };
            }
        }
    }
}