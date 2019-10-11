using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreGame
{
    public class CircleFillBarRendererManager : GameSingleton<CircleFillBarRendererManager>
    {
        #region External Dependencies
        private CoreMaterialConfiguration CoreMaterialConfiguration;
        private CorePrefabConfiguration CorePrefabConfiguration;
        #endregion

        private List<CircleFillBarType> CircleFillBarTypeToRender = new List<CircleFillBarType>();

        public CommandBuffer CommandBuffer { get; private set; }
        private MaterialPropertyBlock materialProperty;

        public void Init()
        {
            this.CommandBuffer = new CommandBuffer();
            this.CommandBuffer.name = this.GetType().Name;
            
            this.materialProperty = new MaterialPropertyBlock();

            this.CorePrefabConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CorePrefabConfiguration;
            this.CoreMaterialConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration;
        }


        public void Tick(float d)
        {
            this.CommandBuffer.Clear();

            foreach (var circleFillBarType in this.CircleFillBarTypeToRender)
            {
                if (circleFillBarType.CurrentProgression != 0f)
                {
                    this.materialProperty.SetFloat(Shader.PropertyToID("_Progression"), circleFillBarType.CurrentProgression);
                    this.CommandBuffer.DrawMesh(this.CorePrefabConfiguration.ForwardQuadMesh, circleFillBarType.transform.localToWorldMatrix, this.CoreMaterialConfiguration.CircleProgressionMaterial, 0, 0, materialProperty);
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
