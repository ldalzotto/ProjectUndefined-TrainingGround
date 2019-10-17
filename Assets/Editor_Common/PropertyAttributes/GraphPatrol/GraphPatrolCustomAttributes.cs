using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class GraphPatrolLineAttribute : Attribute
{
    public string FieldTargetWorldPosition;

    public GraphPatrolLineAttribute(string fieldTargetWorldPosition)
    {
        this.FieldTargetWorldPosition = fieldTargetWorldPosition;
    }
}


[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class GraphPatrolPointAttribute : Attribute
{
    public float R = 1f;
    public float G = 1f;
    public float B = 1f;

    public Color GetColor()
    {
        return new Color(R, G, B);
    }
}