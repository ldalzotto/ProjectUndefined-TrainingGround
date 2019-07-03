﻿using CoreGame;
using System.Collections.Generic;
using UnityEngine;

public class Test_Frustum : MonoBehaviour
{
    public FrustumV2 LocalFrustumV2;

    public Test_Sphere Test_Sphere;

    private bool pointInsideFrustum = false;

    public Material ComparisonTestMaterial;

    private ComputeBuffer FrustumBufferDataBuffer;
    private List<FrustumBufferData> FrustumBufferDatas;

    private Vector3 C1;
    private Vector3 C2;
    private Vector3 C3;
    private Vector3 C4;
    private Vector3 C5;
    private Vector3 C6;
    private Vector3 C7;
    private Vector3 C8;

    private void Start()
    {
        this.FrustumBufferDatas = new List<FrustumBufferData>();
        this.FrustumBufferDataBuffer = new ComputeBuffer(4, FrustumBufferData.GetByteSize());
        this.FrustumBufferDataBuffer.SetData(this.FrustumBufferDatas);
        this.ComparisonTestMaterial.SetBuffer("FrustumBufferDataBuffer", this.FrustumBufferDataBuffer);
    }

    private void OnDisable()
    {
        ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.FrustumBufferDataBuffer);
    }

    private void Update()
    {

        this.FrustumBufferDatas.Clear();
        this.LocalFrustumV2.WorldPosition = this.transform.position;
        this.LocalFrustumV2.LossyScale = this.transform.lossyScale;

        this.LocalFrustumV2.F1.FaceOffsetFromCenter.z = 1;
        this.LocalFrustumV2.Rotation = this.transform.rotation * Quaternion.Euler(0, 0, 0);
        this.LocalFrustumV2.CalculateFrustumPoints(out this.C1, out this.C2, out this.C3, out this.C4, out this.C5, out this.C6, out this.C7, out this.C8);
        this.FrustumBufferDatas.Add(new FrustumBufferData(this.C1, this.C2, this.C3, this.C4, this.C5, this.C6, this.C7, this.C8));
        pointInsideFrustum = Intersection.PointInsideFrustum(this.LocalFrustumV2, this.Test_Sphere.transform.position);

        this.LocalFrustumV2.F1.FaceOffsetFromCenter.z = -1;
        this.LocalFrustumV2.Rotation = this.transform.rotation * Quaternion.Euler(0, 0, 0);
        this.LocalFrustumV2.CalculateFrustumPoints(out this.C1, out this.C2, out this.C3, out this.C4, out this.C5, out this.C6, out this.C7, out this.C8);
        this.FrustumBufferDatas.Add(new FrustumBufferData(this.C1, this.C2, this.C3, this.C4, this.C5, this.C6, this.C7, this.C8));
        pointInsideFrustum = pointInsideFrustum || Intersection.PointInsideFrustum(this.LocalFrustumV2, this.Test_Sphere.transform.position);


        this.LocalFrustumV2.F1.FaceOffsetFromCenter.z = 1;
        this.LocalFrustumV2.Rotation = this.transform.rotation * Quaternion.Euler(0, 90, 0);
        this.LocalFrustumV2.CalculateFrustumPoints(out this.C1, out this.C2, out this.C3, out this.C4, out this.C5, out this.C6, out this.C7, out this.C8);
        this.FrustumBufferDatas.Add(new FrustumBufferData(this.C1, this.C2, this.C3, this.C4, this.C5, this.C6, this.C7, this.C8));
        pointInsideFrustum = pointInsideFrustum || Intersection.PointInsideFrustum(this.LocalFrustumV2, this.Test_Sphere.transform.position);


        this.LocalFrustumV2.F1.FaceOffsetFromCenter.z = -1;
        this.LocalFrustumV2.Rotation = this.transform.rotation * Quaternion.Euler(0, 90, 0);
        this.LocalFrustumV2.CalculateFrustumPoints(out this.C1, out this.C2, out this.C3, out this.C4, out this.C5, out this.C6, out this.C7, out this.C8);
        this.FrustumBufferDatas.Add(new FrustumBufferData(this.C1, this.C2, this.C3, this.C4, this.C5, this.C6, this.C7, this.C8));
        pointInsideFrustum = pointInsideFrustum || Intersection.PointInsideFrustum(this.LocalFrustumV2, this.Test_Sphere.transform.position);


        if (this.ComparisonTestMaterial != null)
        {
            this.ComparisonTestMaterial.SetVector("_SpherePosition", this.Test_Sphere.transform.position);
            this.ComparisonTestMaterial.SetFloat("_SphereRadius", this.Test_Sphere.Radius);

            this.ComparisonTestMaterial.SetFloat("_FrustumBufferDataBufferCount", this.FrustumBufferDatas.Count);
            if (this.FrustumBufferDataBuffer.IsValid())
            {
                this.FrustumBufferDataBuffer.SetData(this.FrustumBufferDatas);
            }
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

[System.Serializable]
public struct FrustumBufferData
{
    public Vector3 FC1;
    public Vector3 FC2;
    public Vector3 FC3;
    public Vector3 FC4;
    public Vector3 FC5;
    public Vector3 FC6;
    public Vector3 FC7;
    public Vector3 FC8;

    public FrustumBufferData(Vector3 fC1, Vector3 fC2, Vector3 fC3, Vector3 fC4, Vector3 fC5, Vector3 fC6, Vector3 fC7, Vector3 fC8)
    {
        FC1 = fC1;
        FC2 = fC2;
        FC3 = fC3;
        FC4 = fC4;
        FC5 = fC5;
        FC6 = fC6;
        FC7 = fC7;
        FC8 = fC8;
    }

    public static int GetByteSize()
    {
        return 8 * 3 * sizeof(float);
    }
}