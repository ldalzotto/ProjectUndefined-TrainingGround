using UnityEngine;
using System.Collections;

public class LineObjTest : MonoBehaviour
{
    public Vector3 LocalStart;
    public Vector3 LocalEnd;

    private Vector3 worldStart;
    private Vector3 worldEnd;

    public Vector3 WorldStart { get => worldStart; }
    public Vector3 WorldEnd { get => worldEnd; }

    private void OnDrawGizmos()
    {
        this.worldStart = this.transform.TransformPoint(this.LocalStart);
        this.worldEnd = this.transform.TransformPoint(this.LocalEnd);

        var oldGizmoColor = Gizmos.color;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.worldStart, this.worldEnd);

        Gizmos.color = oldGizmoColor;
    }
}
