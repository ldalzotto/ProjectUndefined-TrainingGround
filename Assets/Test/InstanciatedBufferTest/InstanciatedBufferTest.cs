using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreGame
{
    public class InstanciatedBufferTest : MonoBehaviour
    {
        public Mesh InstanciatedMesh;
        public Material RenderingMaterial;

        private CommandBuffer commandBuffer;

        private ComputeBuffer argsBuffer;
        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        private List<InstanceBufferData> InstanceBufferDatas;
        private DynamicComputeBufferManager<InstanceBufferData> InstanceBufferDataComputeBuffer;
        void Start()
        {
            this.InstanceBufferDatas = new List<InstanceBufferData>();
            this.InstanceBufferDataComputeBuffer = new DynamicComputeBufferManager<InstanceBufferData>(InstanceBufferData.GetByteSize(), "InstanceBufferDataComputeBuffer", string.Empty, new List<Material>() { this.RenderingMaterial });
            this.argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

            this.commandBuffer = new CommandBuffer();
            Camera.main.AddCommandBuffer(CameraEvent.AfterEverything, this.commandBuffer);
        }

        void Update()
        {
            this.commandBuffer.Clear();
            this.InstanceBufferDatas.Clear();
            this.InstanceBufferDatas = GetComponentsInChildren<Transform>().ToList().ConvertAll(t => new InstanceBufferData() { LocalToWorld = t.localToWorldMatrix });

            args[0] = (uint)InstanciatedMesh.GetIndexCount(0);
            args[1] = (uint)this.InstanceBufferDatas.Count;
            args[2] = (uint)InstanciatedMesh.GetIndexStart(0);
            args[3] = (uint)InstanciatedMesh.GetBaseVertex(0);
            argsBuffer.SetData(args);

            this.InstanceBufferDataComputeBuffer.Tick(Time.deltaTime, (datas) => datas.AddRange(this.InstanceBufferDatas));

            this.commandBuffer.DrawMeshInstancedIndirect(this.InstanciatedMesh, 0, this.RenderingMaterial, 0, argsBuffer);
        }
    }

    public struct InstanceBufferData
    {
        public Matrix4x4 LocalToWorld;

        public static int GetByteSize()
        {
            return (4 * 4) * sizeof(float);
        }
    }
}
