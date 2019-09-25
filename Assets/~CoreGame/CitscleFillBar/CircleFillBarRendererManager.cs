using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreGame
{
    public class CircleFillBarRendererManager : MonoBehaviour
    {
        public Material CircleFillBarMaterial;

        private List<CircleFillBarType> CircleFillBarTypeToRender = new List<CircleFillBarType>();

        private CommandBuffer commandBuffer;
        private MaterialPropertyBlock materialProperty;

        public void Init()
        {
            this.commandBuffer = new CommandBuffer();
            this.commandBuffer.name = this.GetType().Name;

            Camera.main.AddCommandBuffer(CameraEvent.AfterEverything, this.commandBuffer);
            this.materialProperty = new MaterialPropertyBlock();
        }


        public void Tick(float d)
        {
            this.commandBuffer.Clear();

            foreach (var circleFillBarType in this.CircleFillBarTypeToRender)
            {
                if (circleFillBarType.CurrentProgression != 0f)
                {
                    this.materialProperty.SetFloat(Shader.PropertyToID("_Progression"), circleFillBarType.CurrentProgression);
                    this.commandBuffer.DrawMesh(circleFillBarType.MeshFilter.mesh, circleFillBarType.transform.localToWorldMatrix, this.CircleFillBarMaterial, 0, 0, materialProperty);
                }
            }
        }

        #region External Event
        public void OnCircleFillBarTypeCreated(CircleFillBarType CircleFillBarTypeRef)
        {
            this.CircleFillBarTypeToRender.Add(CircleFillBarTypeRef);
        }

        public void OnCircleFillBarTypeDestroyed(CircleFillBarType CircleFillBarTypeRef)
        {
            this.CircleFillBarTypeToRender.Remove(CircleFillBarTypeRef);
        }
        #endregion
    }
}
