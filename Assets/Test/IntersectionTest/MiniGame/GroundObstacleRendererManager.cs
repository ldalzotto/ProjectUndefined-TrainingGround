using System.Collections.Generic;
using UnityEngine;

public class GroundObstacleRendererManager : MonoBehaviour
{

    #region External Dependencies
    private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
    private ObstaclesListenerManager ObstaclesListenerManager;
    #endregion

    public Material GroundFrustumMaterial;

    private DynamicComputeBufferManager<FrustumPointsWorldPositions> FrustumBufferManager;
    private DynamicComputeBufferManager<FrustumProjectionPointBufferData> FrustumProjectionPointBufferManager;
    private DynamicComputeBufferManager<FrustumPositionToProjectionPositionLinkTable> FrustumPositionToProjectionPositionLinkTableBufferManager;

    private List<FrustumPositionToProjectionPositionLinkTable> frustumBufferLinkDatas;

    private List<Color> Colors = new List<Color>() {
        Color.green,
        Color.red,
        Color.blue
    };

    public void Init()
    {
        this.ObstacleFrustumCalculationManager = GameObject.FindObjectOfType<ObstacleFrustumCalculationManager>();
        this.ObstaclesListenerManager = GameObject.FindObjectOfType<ObstaclesListenerManager>();

        this.FrustumBufferManager = new DynamicComputeBufferManager<FrustumPointsWorldPositions>(FrustumPointsWorldPositions.GetByteSize(), "FrustumBufferDataBuffer", "_FrustumBufferDataBufferCount", ref this.GroundFrustumMaterial);
        this.FrustumProjectionPointBufferManager = new DynamicComputeBufferManager<FrustumProjectionPointBufferData>(FrustumProjectionPointBufferData.GetByteSize(), "FrustumProjectionPointBufferDataBuffer", "_FrustumProjectionPointBufferCount", ref this.GroundFrustumMaterial);
        this.FrustumPositionToProjectionPositionLinkTableBufferManager =
            new DynamicComputeBufferManager<FrustumPositionToProjectionPositionLinkTable>(FrustumPositionToProjectionPositionLinkTable.GetByteSize(), "FrustumPositionToProjectionPositionLinkTableBuffer", string.Empty, ref this.GroundFrustumMaterial);
        this.frustumBufferLinkDatas = new List<FrustumPositionToProjectionPositionLinkTable>();
    }


    public void Tick(float d)
    {
        this.FrustumProjectionPointBufferManager.Tick(d, (List<FrustumProjectionPointBufferData> frustumProjectionPointBufferDatas) =>
        {
            foreach (var testSphere in this.ObstaclesListenerManager.GetAllObstacleListeners())
            {
                frustumProjectionPointBufferDatas.Add(new FrustumProjectionPointBufferData(testSphere.transform.position, testSphere.Radius, this.Colors[this.ObstaclesListenerManager.GetAllObstacleListeners().IndexOf(testSphere)]));
            }
        });

        this.frustumBufferLinkDatas.Clear();
        this.FrustumBufferManager.Tick(d, (List<FrustumPointsWorldPositions> frustumBufferDatas) =>
        {
            foreach (var testSphere in this.ObstaclesListenerManager.GetAllObstacleListeners())
            {
                foreach (var nearObstable in testSphere.NearSquereObstacles)
                {
                    var frustumCalculationResults = this.ObstacleFrustumCalculationManager.GetResult(testSphere, nearObstable).CalculatedFrustumPositions;

                    frustumBufferDatas.AddRange(frustumCalculationResults);

                    var obstacleListenerIndex = this.ObstaclesListenerManager.GetAllObstacleListeners().IndexOf(testSphere);
                    for (var i = 0; i < frustumCalculationResults.Count; i++)
                    {
                        frustumBufferLinkDatas.Add(new FrustumPositionToProjectionPositionLinkTable(obstacleListenerIndex));
                    }

                }
            }
        });
        this.FrustumPositionToProjectionPositionLinkTableBufferManager.Tick(d, (List<FrustumPositionToProjectionPositionLinkTable> frustumBufferLinkDatasToSet) =>
        {
            frustumBufferLinkDatasToSet.AddRange(frustumBufferLinkDatas);
        });
        
    }

    private void OnApplicationQuit()
    {
        this.FrustumBufferManager.Dispose();
        this.FrustumProjectionPointBufferManager.Dispose();
        this.FrustumPositionToProjectionPositionLinkTableBufferManager.Dispose();
    }
}

[System.Serializable]
public struct FrustumPositionToProjectionPositionLinkTable
{
    public int FrustumProjectionPointBufferDataIndex;

    public FrustumPositionToProjectionPositionLinkTable(int frustumProjectionPointBufferDataIndex)
    {
        FrustumProjectionPointBufferDataIndex = frustumProjectionPointBufferDataIndex;
    }

    public static int GetByteSize()
    {
        return (1) * sizeof(int);
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