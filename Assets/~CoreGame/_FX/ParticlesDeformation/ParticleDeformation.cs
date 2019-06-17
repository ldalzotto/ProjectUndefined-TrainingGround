using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class ParticleDeformation : MonoBehaviour
    {

        public float DeformationRadius;
        public float DeformationStrength;

        public bool EveryFrame = true;
        public float BufferUpdateRateInSeconds;

        public MeshRenderer TargetMeshRenderer;

        private ParticleSystem ParticleSystem;

        private ComputeBuffer ParticleDeformationBuffer;
        private float ParticleCount;
        private List<ParticleDeformationBufferData> ParticleDeformationBufferDatas;

        private ParticleSystem.Particle[] StoredParticles;

        private float timeCounter;
        private bool isVisible;

        void Start()
        {
         //   this.TargetMeshRenderer = GetComponent<MeshRenderer>();
            this.ParticleSystem = GetComponent<ParticleSystem>();
            this.ParticleDeformationBufferDatas = new List<ParticleDeformationBufferData>();
            this.InitComputeBuffer();
            this.StoredParticles = new ParticleSystem.Particle[this.ParticleSystem.main.maxParticles];
        }

        private void InitComputeBuffer()
        {
            if (this.ParticleDeformationBuffer == null || !this.ParticleDeformationBuffer.IsValid())
            {
                this.ParticleDeformationBuffer = new ComputeBuffer(this.ParticleSystem.main.maxParticles, (3 + 1 + 1) * sizeof(float));
                this.ParticleDeformationBuffer.SetData(this.ParticleDeformationBufferDatas);
                this.TargetMeshRenderer.material.SetBuffer("_ParticleDeformationBuffer", this.ParticleDeformationBuffer);
            }
        }

        void Update()
        {
            if (this.ParticleDeformationBuffer.IsValid())
            {
                if (this.EveryFrame)
                {
                    this.UpdateBuffer();
                }
                else
                {
                    this.timeCounter += Time.deltaTime;
                    if (this.timeCounter >= this.BufferUpdateRateInSeconds)
                    {
                        this.UpdateBuffer();
                        this.timeCounter -= this.BufferUpdateRateInSeconds;
                    }
                }
            }

        }

        private void UpdateBuffer()
        {
            this.ParticleDeformationBufferDatas.Clear();

            this.ParticleSystem.GetParticles(this.StoredParticles);
            for (var i = 0; i < this.ParticleSystem.particleCount; i++)
            {
                var currentParticle = this.StoredParticles[i];
                this.ParticleDeformationBufferDatas.Add(new ParticleDeformationBufferData(this.ParticleSystem.transform.TransformPoint(currentParticle.position), DeformationRadius, DeformationStrength));
            }

            this.TargetMeshRenderer.material.SetInt("_ParticleDeformationBufferCount", this.ParticleDeformationBufferDatas.Count);
            this.ParticleDeformationBuffer.SetData(this.ParticleDeformationBufferDatas);
        }

        private void OnDrawGizmos()
        {
            if (this.ParticleDeformationBufferDatas != null)
            {
                foreach (var particleBufferData in this.ParticleDeformationBufferDatas)
                {
                    Gizmos.DrawWireSphere(particleBufferData.WolrdPosition, 1f);
                }
            }
            
        }

        private void OnBecameVisible()
        {
            this.isVisible = true;
            this.InitComputeBuffer();
        }
        private void OnBecameInvisible()
        {
            this.isVisible = false;
            this.OnDestroy();
        }

        private void OnDestroy()
        {
            this.ParticleDeformationBuffer.Release();
            this.ParticleDeformationBuffer.Dispose();
        }
    }

    public struct ParticleDeformationBufferData
    {
        public Vector3 WolrdPosition;
        public float DeformationRadius;
        public float DeformationStrength;

        public ParticleDeformationBufferData(Vector3 wolrdPosition, float deformationRadius, float deformationStrength)
        {
            WolrdPosition = wolrdPosition;
            DeformationRadius = deformationRadius;
            DeformationStrength = deformationStrength;
        }
    }

}
