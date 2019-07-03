using CoreGame;
using UnityEngine;

[ExecuteInEditMode]
public class Test_Frustum : MonoBehaviour
{
    public FrustumV2 LocalFrustumV2;

    public Test_Sphere Test_Sphere;

    private bool pointInsideFrustum = false;

    public Material ComparisonTestMaterial;
    private Vector3 C1;
    private Vector3 C2;
    private Vector3 C3;
    private Vector3 C4;
    private Vector3 C5;
    private Vector3 C6;
    private Vector3 C7;
    private Vector3 C8;

    private void Update()
    {
        this.LocalFrustumV2.WorldPosition = this.transform.position;
        this.LocalFrustumV2.Rotation = this.transform.rotation;
        this.LocalFrustumV2.LossyScale = this.transform.lossyScale;

        this.LocalFrustumV2.CalculateFrustumPoints(out this.C1, out this.C2, out this.C3, out this.C4, out this.C5, out this.C6, out this.C7, out this.C8);

        #region FRUSTUM<->SPHERE
        //  bool pointInsideFrustum = false;


        /*
        Vector3 normal = -Vector3.Cross(C2 - C1, C3 - C1);
        pointInsideFrustum = (Vector3.Dot(normal, this.Test_Sphere.transform.position - C1) >= 0);
        Gizmos.DrawLine(C1, C1 + normal);

        if (pointInsideFrustum)
        {
            normal = -Vector3.Cross(C5 - C1, C2 - C1);
            pointInsideFrustum = (Vector3.Dot(normal, this.Test_Sphere.transform.position - C1) >= 0);
            Gizmos.DrawLine(C1, C1 + normal);

            if (pointInsideFrustum)
            {
                normal = -Vector3.Cross(C6 - C2, C3 - C2);
                pointInsideFrustum = (Vector3.Dot(normal, this.Test_Sphere.transform.position - C2) >= 0);
                Gizmos.DrawLine(C2, C2 + normal);

                if (pointInsideFrustum)
                {
                    normal = -Vector3.Cross(C7 - C3, C4 - C3);
                    pointInsideFrustum = (Vector3.Dot(normal, this.Test_Sphere.transform.position - C3) >= 0);
                    Gizmos.DrawLine(C3, C3 + normal);

                    if (pointInsideFrustum)
                    {
                        normal = -Vector3.Cross(C8 - C4, C1 - C4);
                        pointInsideFrustum = (Vector3.Dot(normal, this.Test_Sphere.transform.position - C4) >= 0);
                        Gizmos.DrawLine(C4, C4 + normal);

                        if (pointInsideFrustum)
                        {
                            normal = -Vector3.Cross(C8 - C5, C6 - C5);
                            pointInsideFrustum = (Vector3.Dot(normal, this.Test_Sphere.transform.position - C5) >= 0);
                            Gizmos.DrawLine(C5, C5 + normal);
                        }
                    }
                }
            }
        }
        */
        /*
        frustumIntersectSphere = TestIntersectionWithGizmo(C1, C2, C3, C4);
        if (!frustumIntersectSphere)
        {
            frustumIntersectSphere = TestIntersectionWithGizmo(C1, C5, C6, C2);
            if (!frustumIntersectSphere)
            {
                frustumIntersectSphere = TestIntersectionWithGizmo(C2, C6, C7, C3);
                if (!frustumIntersectSphere)
                {
                    frustumIntersectSphere = TestIntersectionWithGizmo(C4, C8, C7, C3);
                    if (!frustumIntersectSphere)
                    {
                        frustumIntersectSphere = TestIntersectionWithGizmo(C4, C1, C5, C8);
                        if (!frustumIntersectSphere)
                        {
                            frustumIntersectSphere = TestIntersectionWithGizmo(C5, C6, C7, C8);
                        }
                    }
                }
            }
        }
        */
        #endregion

        pointInsideFrustum = Intersection.PointInsideFrustum(this.LocalFrustumV2, this.Test_Sphere.transform.position);


        if (this.ComparisonTestMaterial != null)
        {
            this.ComparisonTestMaterial.SetVector("_FC1", C1);
            this.ComparisonTestMaterial.SetVector("_FC2", C2);
            this.ComparisonTestMaterial.SetVector("_FC3", C3);
            this.ComparisonTestMaterial.SetVector("_FC4", C4);
            this.ComparisonTestMaterial.SetVector("_FC5", C5);
            this.ComparisonTestMaterial.SetVector("_FC6", C6);
            this.ComparisonTestMaterial.SetVector("_FC7", C7);
            this.ComparisonTestMaterial.SetVector("_FC8", C8);

            this.ComparisonTestMaterial.SetVector("_SpherePosition", this.Test_Sphere.transform.position);
            this.ComparisonTestMaterial.SetFloat("_SphereRadius", this.Test_Sphere.Radius);
        }
    }

    private void OnDrawGizmos()
    {
        DrawFace(C1, C2, C3, C4);
        DrawFace(C1, C5, C6, C2);
        DrawFace(C2, C6, C7, C3);
        DrawFace(C4, C8, C7, C3);
        DrawFace(C4, C1, C5, C8);
        DrawFace(C5, C6, C7, C8);
        
        var oldGizmoColor = Gizmos.color;
        if (pointInsideFrustum)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(this.Test_Sphere.transform.position, 0.1f);
        Gizmos.DrawWireSphere(this.transform.position + this.LocalFrustumV2.LocalStartAngleProjection, 0.1f);
        Gizmos.color = oldGizmoColor;

    }

    #region FRUSTUM<->SPHERE
    /*
    private bool TestIntersectionWithGizmo(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
    {
        bool frustumIntersectSphere = Intersection.FaceIntersectOrContainsSphere(C1, C2, C3, C4, Test_Sphere.transform.position, Test_Sphere.Radius);
        if (frustumIntersectSphere)
        {
            Gizmos.DrawWireSphere(C1, 0.1f);
            Gizmos.DrawWireSphere(C2, 0.1f);
            Gizmos.DrawWireSphere(C3, 0.1f);
            Gizmos.DrawWireSphere(C4, 0.1f);
        }

        return frustumIntersectSphere;
    }
    */
    #endregion

    private void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
    {
        Gizmos.DrawLine(C1, C2);
        Gizmos.DrawLine(C2, C3);
        Gizmos.DrawLine(C3, C4);
        Gizmos.DrawLine(C4, C1);
    }
}
