using UnityEngine;

public class BeziersControlPoints
{
    private Vector3 p0;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    public Vector3 P0 { get => p0; set => p0 = value; }
    public Vector3 P1 { get => p1; set => p1 = value; }
    public Vector3 P2 { get => p2; set => p2 = value; }
    public Vector3 P3 { get => p3; set => p3 = value; }

    public Vector3 ResolvePoint(float t)
    {
        return p0 * Mathf.Pow((1 - t), 3) + (3 * p1 * t * Mathf.Pow(1 - t, 2)) + (3 * p2 * Mathf.Pow(t, 2) * (1 - t)) + (p3 * Mathf.Pow(t, 3));
    }

    public void GizmoSelectedTick()
    {
        if (p0 != null)
        {
            Gizmos.DrawWireSphere(p0, 0.4f);
        }
        if (p1 != null)
        {
            Gizmos.DrawWireSphere(p1, 0.6f);
        }
        if (p2 != null)
        {
            Gizmos.DrawWireSphere(p2, 0.8f);
        }
        if (p3 != null)
        {
            Gizmos.DrawWireSphere(p3, 1f);
        }
    }

    public BeziersControlPoints Clone()
    {
        var beziersClone = new BeziersControlPoints();
        beziersClone.p0 = p0;
        beziersClone.p1 = p1;
        beziersClone.p2 = p2;
        beziersClone.p3 = p3;
        return beziersClone;
    }
}
