using UnityEngine;
using System.Collections;
using UnityEditor;

public class HandlesHelper
{

    public static void DrawBoxCollider(BoxCollider boxCollider, Transform transform, Color color, string label, GUIStyle labelStyle)
    {
        var oldColor = Handles.color;
        Handles.color = color;
        Handles.Label(transform.TransformPoint(new Vector3(boxCollider.center.x, boxCollider.center.y + boxCollider.bounds.max.y + 10f, boxCollider.center.z)), label, labelStyle);
        Handles.DrawWireCube(transform.TransformPoint(boxCollider.center), boxCollider.size);
        Handles.color = oldColor;
    }
}
