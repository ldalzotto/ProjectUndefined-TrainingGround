using CoreGame;
using System.Collections.Generic;
using UnityEngine;

public class SquareObstacle : MonoBehaviour
{
    public FrustumV2 LocalFrustumV2;

    public static SquareObstacle FromCollisionType(CollisionType collisionType)
    {
        if (collisionType.IsObstacle)
        {
            return collisionType.GetComponent<SquareObstacle>();
        }
        return null;
    }
    
    public List<FrustumBufferData> ComputeOcclusionFrustums(Vector3 worldPositionStartAngleDefinition)
    {
        List<FrustumBufferData> frustumBufferDatas = new List<FrustumBufferData>();

        this.LocalFrustumV2.SetLocalStartAngleProjection(worldPositionStartAngleDefinition);
        this.LocalFrustumV2.WorldPosition = this.transform.position;
        this.LocalFrustumV2.LossyScale = this.transform.lossyScale;

        ComputeSideFrustum(frustumBufferDatas, Quaternion.Euler(0, 0, 0), 1);
        ComputeSideFrustum(frustumBufferDatas, Quaternion.Euler(0, 0, 0), -1);
        ComputeSideFrustum(frustumBufferDatas, Quaternion.Euler(0, 90, 0), 1);
        ComputeSideFrustum(frustumBufferDatas, Quaternion.Euler(0, 90, 0), -1);

        /*
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
        */

        return frustumBufferDatas;
    }

    private void ComputeSideFrustum(List<FrustumBufferData> frustumBufferDatas, Quaternion sideDeltaRotation, float zPositionOffset)
    {
        this.LocalFrustumV2.F1.FaceOffsetFromCenter.z = zPositionOffset;
        this.LocalFrustumV2.Rotation = this.transform.rotation * sideDeltaRotation;
        this.LocalFrustumV2.CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
        frustumBufferDatas.Add(new FrustumBufferData(C1, C2, C3, C4, C5, C6, C7, C8));
    }
}
