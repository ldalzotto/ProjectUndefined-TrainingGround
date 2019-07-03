using CoreGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundObstacleRendererManager : MonoBehaviour
{

    public Material GroundFrustumMaterial;

    private List<Test_Sphere> SphereTrackers;

    private ComputeBuffer FrustumBufferDataBuffer;
    private List<FrustumBufferData> FrustumBufferDatas;

    public void Init()
    {
        this.SphereTrackers = new List<Test_Sphere>();
        this.FrustumBufferDatas = new List<FrustumBufferData>();
        this.FrustumBufferDataBuffer = new ComputeBuffer(4, FrustumBufferData.GetByteSize());
        this.FrustumBufferDataBuffer.SetData(this.FrustumBufferDatas);
        this.GroundFrustumMaterial.SetBuffer("FrustumBufferDataBuffer", this.FrustumBufferDataBuffer);
    }

    #region External Event
    public void OntestSphereCreation(Test_Sphere Test_Sphere)
    {
        this.SphereTrackers.Add(Test_Sphere);
    }
    #endregion

    public void Tick(float d)
    {
        this.FrustumBufferDatas.Clear();


        var frustumCount = this.SphereTrackers.SelectMany(i => i.NearSquereObstacles).Count() * 4;
        if (this.FrustumBufferDataBuffer.count != frustumCount)
        {
            if (frustumCount != 0)
            {
                //    this.FrustumBufferDataBuffer.SetCounterValue((uint)frustumCount);
                this.OnApplicationQuit();
                this.FrustumBufferDataBuffer = new ComputeBuffer(frustumCount, FrustumBufferData.GetByteSize());
                this.GroundFrustumMaterial.SetBuffer("FrustumBufferDataBuffer", this.FrustumBufferDataBuffer);
            }
        }

        foreach (var testSphere in this.SphereTrackers)
        {
            this.GroundFrustumMaterial.SetVector("_SpherePosition", testSphere.transform.position);
            this.GroundFrustumMaterial.SetFloat("_SphereRadius", testSphere.Radius);

            foreach(var nearObstable in testSphere.NearSquereObstacles)
            {
                this.FrustumBufferDatas.AddRange(nearObstable.ComputeOcclusionFrustums(testSphere.transform.position));
            }

      //      this.FrustumBufferDatas.AddRange(testSphere.NearSquereObstacles.ConvertAll(obs => obs.ComputeOcclusionFrustums(testSphere.transform.position)).SelectMany(e => e));
        }

        this.GroundFrustumMaterial.SetFloat("_FrustumBufferDataBufferCount", this.FrustumBufferDatas.Count);
        this.FrustumBufferDataBuffer.SetData(this.FrustumBufferDatas);
    }

    private void OnApplicationQuit()
    {
        ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.FrustumBufferDataBuffer);
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
