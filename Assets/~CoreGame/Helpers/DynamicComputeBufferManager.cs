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
        private Material material;
        private string materialPropertyName;
        private string materialCountPropertyName;
        private int objectByteSize;

        public DynamicComputeBufferManager(int objectByteSize, string materialPropertyName, string materialCountPropertyName, ref Material material , BufferReAllocateStrategy BufferReAllocateStrategy = BufferReAllocateStrategy.NONE)
        {
            this.objectByteSize = objectByteSize;
            this.materialPropertyName = materialPropertyName;
            this.materialCountPropertyName = materialCountPropertyName;
            this.material = material;
            this.BufferReAllocateStrategy = BufferReAllocateStrategy;

            this.customComputeBuffer = new ComputeBuffer(1, objectByteSize);
            this.computeBufferData = new List<T>();
            this.material.SetBuffer(this.materialPropertyName, this.customComputeBuffer);
        }

        public void Tick(float d, Action<List<T>> bufferDataSetter)
        {
            if (this.customComputeBuffer != null && this.customComputeBuffer.IsValid())
            {
                this.computeBufferData.Clear();
                bufferDataSetter.Invoke(this.computeBufferData);

                if ( (this.BufferReAllocateStrategy == BufferReAllocateStrategy.NONE && this.customComputeBuffer.count != this.computeBufferData.Count) 
                      || (this.BufferReAllocateStrategy == BufferReAllocateStrategy.SUPERIOR_ONLY && this.computeBufferData.Count > this.customComputeBuffer.count ))
                {
                    if (this.computeBufferData.Count != 0)
                    {
                        this.Dispose();
                        this.customComputeBuffer = new ComputeBuffer(this.computeBufferData.Count, this.objectByteSize);
                        this.material.SetBuffer(this.materialPropertyName, this.customComputeBuffer);
                    }
                }

                if (!string.IsNullOrEmpty(this.materialCountPropertyName))
                {
                    this.material.SetFloat(this.materialCountPropertyName, this.computeBufferData.Count);
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
