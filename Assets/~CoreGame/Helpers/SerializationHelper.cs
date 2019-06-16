using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Vector3Binarry
{
    [SerializeField]
    public float x;
    [SerializeField]
    public float y;
    [SerializeField]
    public float z;

    public Vector3Binarry(Vector3 vector3) 
    {
        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;
    }
}

[System.Serializable]
public struct QuaternionBinarry
{
    [SerializeField]
    public float x;
    [SerializeField]
    public float y;
    [SerializeField]
    public float z;
    [SerializeField]
    public float w;

    public QuaternionBinarry(Quaternion quaternion)
    {
        this.x = quaternion.x;
        this.y = quaternion.y;
        this.z = quaternion.z;
        this.w = quaternion.w;
    }
}