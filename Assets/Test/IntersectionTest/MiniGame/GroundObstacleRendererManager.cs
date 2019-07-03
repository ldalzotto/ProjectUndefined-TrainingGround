using System.Collections.Generic;
using UnityEngine;

public class GroundObstacleRendererManager : MonoBehaviour
{

    public Material GroundFrustumMaterial;

    private List<Test_Sphere> SphereTrackers;

    private DynamicComputeBufferManager<FrustumBufferData> FrustumBufferManager;
    private DynamicComputeBufferManager<FrustumProjectionPointBufferData> FrustumProjectionPointBufferManager;

    private List<Color> Colors = new List<Color>() {
        Color.green,
        Color.red,
        Color.blue
    };

    public void Init()
    {
        this.SphereTrackers = new List<Test_Sphere>();
        this.FrustumBufferManager = new DynamicComputeBufferManager<FrustumBufferData>(FrustumBufferData.GetByteSize(), "FrustumBufferDataBuffer", "_FrustumBufferDataBufferCount", ref this.GroundFrustumMaterial);
        this.FrustumProjectionPointBufferManager = new DynamicComputeBufferManager<FrustumProjectionPointBufferData>(FrustumProjectionPointBufferData.GetByteSize(), "FrustumProjectionPointBufferDataBuffer", "_FrustumProjectionPointBufferCount", ref this.GroundFrustumMaterial);
    }

    #region External Event
    public void OntestSphereCreation(Test_Sphere Test_Sphere)
    {
        this.SphereTrackers.Add(Test_Sphere);
    }
    #endregion

    public void Tick(float d)
    {
        this.FrustumProjectionPointBufferManager.Tick(d, (List<FrustumProjectionPointBufferData> frustumProjectionPointBufferDatas) =>
        {
            foreach (var testSphere in this.SphereTrackers)
            {
                frustumProjectionPointBufferDatas.Add(new FrustumProjectionPointBufferData(testSphere.transform.position, testSphere.Radius, this.Colors[this.SphereTrackers.IndexOf(testSphere)]));
            }
        });
        this.FrustumBufferManager.Tick(d, (List<FrustumBufferData> frustumBufferDatas) =>
        {
            foreach (var testSphere in this.SphereTrackers)
            {
                var FrustumProjectionPointBufferDataIndex = this.SphereTrackers.IndexOf(testSphere);
                foreach (var nearObstable in testSphere.NearSquereObstacles)
                {
                    frustumBufferDatas.AddRange(nearObstable.ComputeOcclusionFrustums(testSphere.transform.position, FrustumProjectionPointBufferDataIndex));
                }
            }
        });
        
    }

    private void OnApplicationQuit()
    {
        this.FrustumBufferManager.Dispose();
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
    public int FrustumProjectionPointBufferDataIndex;

    public FrustumBufferData(Vector3 fC1, Vector3 fC2, Vector3 fC3, Vector3 fC4, Vector3 fC5, Vector3 fC6, Vector3 fC7, Vector3 fC8, int FrustumProjectionPointBufferDataIndex)
    {
        FC1 = fC1;
        FC2 = fC2;
        FC3 = fC3;
        FC4 = fC4;
        FC5 = fC5;
        FC6 = fC6;
        FC7 = fC7;
        FC8 = fC8;
        this.FrustumProjectionPointBufferDataIndex = FrustumProjectionPointBufferDataIndex;
    }
    
    public static int GetByteSize()
    {
        return (8 * 3 * sizeof(float)) + (1 * sizeof(int));
    }
}

[System.Serializable]
public struct FrustumProjectionPointBufferData
{
    public Vector3 WorldPosition;
    public float Radius;
    public Color BaseColor;

    public FrustumProjectionPointBufferData(Vector3 worldPosition, float radius, Color baseColor)
    {
        WorldPosition = worldPosition;
        Radius = radius;
        BaseColor = baseColor;
    }

    public static int GetByteSize()
    {
        return ((1 * 3) + (1 * 4) + 1) * sizeof(float);
    }
}