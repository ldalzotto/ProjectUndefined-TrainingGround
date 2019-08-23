using OdinSerializer;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public abstract class RangeShapeConfiguration : SerializedScriptableObject
    {
#if UNITY_EDITOR
        public virtual void HandleDraw(Vector3 worldPosition, Quaternion worldRotation, Vector3 lossyScale) { }
#endif
    }
}
