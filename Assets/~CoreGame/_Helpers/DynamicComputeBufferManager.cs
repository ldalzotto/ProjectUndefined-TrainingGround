using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class DynamicComputeBufferManager<T> where T : struct
    {
        private ComputeBuffer customComputeBuffer;
        private List<T> computeBufferData;

        private BufferReAllocateStrategy BufferReAllocateStrategy;
        private List<Material> materials;
        private string materialPropertyName;
        private string materialCountPropertyName;
        private int objectByteSize;

        public DynamicComputeBufferManager(int objectByteSize, string materialPropertyName, string materialCountPropertyName, List<Material> materials, BufferReAllocateStrategy BufferReAllocateStrategy = BufferReAllocateStrategy.NONE, ComputeBufferType ComputeBufferType = ComputeBufferType.Default)
        {
            this.objectByteSize = objectByteSize;
            this.materialPropertyName = materialPropertyName;
            this.materialCountPropertyName = materialCountPropertyName;
            this.materials = materials;
            this.BufferReAllocateStrategy = BufferReAllocateStrategy;

            this.customComputeBuffer = new ComputeBuffer(1, objectByteSize, ComputeBufferType);
            this.computeBufferData = new List<T>();
            foreach (var material in this.materials)
            {
                material.SetBuffer(this.materialPropertyName, this.customComputeBuffer);
            }
        }

        public void Tick(float d, Action<List<T>> bufferDataSetter)
        {
            if (this.customComputeBuffer != null && this.customComputeBuffer.IsValid())
            {
                this.computeBufferData.Clear();
                bufferDataSetter.Invoke(this.computeBufferData);

                if ((this.BufferReAllocateStrategy == BufferReAllocateStrategy.NONE && this.customComputeBuffer.count != this.computeBufferData.Count)
                      || (this.BufferReAllocateStrategy == BufferReAllocateStrategy.SUPERIOR_ONLY && this.computeBufferData.Count > this.customComputeBuffer.count))
                {
                    if (this.computeBufferData.Count != 0)
                    {
                        this.Dispose();
                        this.customComputeBuffer = new ComputeBuffer(this.computeBufferData.Count, this.objectByteSize);
                        foreach (var material in this.materials)
                        {
                            material.SetBuffer(this.materialPropertyName, this.customComputeBuffer);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(this.materialCountPropertyName))
                {
                    foreach (var material in this.materials)
                    {
                        material.SetFloat(this.materialCountPropertyName, this.computeBufferData.Count);
                    }
                }
                this.customComputeBuffer.SetData(this.computeBufferData);
            }

        }

        public void Dispose()
        {
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.customComputeBuffer);
        }
    }

    public enum BufferReAllocateStrategy
    {
        NONE = 0,
        SUPERIOR_ONLY = 1
    }

}
