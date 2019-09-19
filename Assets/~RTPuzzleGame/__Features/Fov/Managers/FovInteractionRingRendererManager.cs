using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class FovInteractionRingRendererManager : MonoBehaviour
    {
        
        private FovInteractionRingCommandBufferManager FovInteractionRingCommandBufferManager;

        #region External Dependencies
        private FovInteractionRingContainer NpcInteractionRingContainer;
        #endregion

        public void Init()
        {
            this.FovInteractionRingCommandBufferManager = new FovInteractionRingCommandBufferManager(CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration,
                    Camera.main);
            this.NpcInteractionRingContainer = PuzzleGameSingletonInstances.NpcInteractionRingContainer;
        }

        public void Tick(float d)
        {
            this.FovInteractionRingCommandBufferManager.Tick(d, NpcInteractionRingContainer.ActiveNpcInteractionRings);
        }

    }

    class FovInteractionRingCommandBufferManager
    {
        private Camera mainCamera;
        private CoreMaterialConfiguration CoreMaterialConfiguration;

        public FovInteractionRingCommandBufferManager(CoreMaterialConfiguration CoreMaterialConfiguration, Camera mainCamera)
        {
            this.CoreMaterialConfiguration = CoreMaterialConfiguration;
            this.mainCamera = mainCamera;
            this.interactionRingBuffer = new CommandBuffer();
            this.interactionRingBuffer.name = "Interaction Ring Render";

            this.mainCamera.AddCommandBuffer(CameraEvent.AfterEverything, this.interactionRingBuffer);
        }

        private CommandBuffer interactionRingBuffer;

        public void Tick(float d, HashSet<FovInteractionRingType> activeNpcInteractionRings)
        {
            this.interactionRingBuffer.Clear();
            if (activeNpcInteractionRings.Count > 0)
            {
                var materialProperty = new MaterialPropertyBlock();
                foreach (var interactionRing in activeNpcInteractionRings)
                {
                    materialProperty.SetTexture("_MainTex", interactionRing.RingTexture);
                    this.interactionRingBuffer.DrawMesh(interactionRing.MeshFilter.mesh, interactionRing.MeshRenderer.localToWorldMatrix, this.CoreMaterialConfiguration.InteractionRingMaterial, 0, 0, materialProperty);
                }
            }
        }

    }
}
