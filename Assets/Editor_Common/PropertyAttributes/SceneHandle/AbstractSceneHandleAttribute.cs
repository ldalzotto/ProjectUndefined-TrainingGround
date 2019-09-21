using UnityEngine;
using System.Collections;

public abstract class AbstractSceneHandleAttribute : PropertyAttribute
{
    public float R = 1f;
    public float G = 0f;
    public float B = 0f;
    
    public Color GetColor()
    {
        return new Color(this.R, this.G, this.B);
    }
}
