using UnityEngine;
using System.Collections;

[System.Serializable]
public struct TransformStruct
{
    public Vector3 WorldPosition;
    public Quaternion WorldRotation;
    public Vector3 LossyScale;

    public TransformStruct(Transform transform)
    {
        this.WorldPosition = transform.position;
        this.WorldRotation = transform.rotation;
        this.LossyScale = transform.lossyScale;
    }

    public bool IsEqualTo(TransformStruct other)
    {
        return (this.WorldPosition == other.WorldPosition) && (this.WorldRotation == other.WorldRotation) && (this.LossyScale == other.LossyScale);
    }
}