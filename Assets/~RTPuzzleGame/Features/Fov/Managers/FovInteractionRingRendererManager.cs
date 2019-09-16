using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class FovInteractionRingRendererManager : MonoBehaviour
    {

        public FovInteractionRingCommandBufferManagerComponent FovInteractionRingCommandBufferManagerComponent;

        private FovInteractionRingCommandBufferManager FovInteractionRingCommandBufferManager;

        #region External Dependencies
        private FovInteractionRingContainer NpcInteractionRingContainer;
        #endregion

        public void Init()
        {
            this.FovInteractionRingCommandBufferManager = new FovInteractionRingCommandBufferManager(FovInteractionRingCommandBufferManagerComponent, Camera.main);
            this.NpcInteractionRingContainer = PuzzleGameSingletonInstances.NpcInteractionRingContainer;
        }

        public void Tick(float d)
        {
            this.FovInteractionRingCommandBufferManager.Tick(d, NpcInteractionRingContainer.ActiveNpcInteractionRings);
        }

    }

    [System.Serializable]
    public class FovInteractionRingCommandBufferManagerComponent
    {
        public Material shaderInteractionRingMaterial;
    }

    class FovInteractionRingCommandBufferManager
    {
        private FovInteractionRingCommandBufferManagerComponent FovInteractionRingCommandBufferManagerComponent;
        private Camera mainCamera;

        public FovInteractionRingCommandBufferManager(FovInteractionRingCommandBufferManagerComponent FovInteractionRingCommandBufferManagerComponent, Camera mainCamera)
        {
            this.FovInteractionRingCommandBufferManagerComponent = FovInteractionRingCommandBufferManagerComponent;
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
                    this.interactionRingBuffer.DrawMesh(interactionRing.MeshFilter.mesh, interactionRing.MeshRenderer.localToWorldMatrix, FovInteractionRingCommandBufferManagerComponent.shaderInteractionRingMaterial, 0, 0, materialProperty);
                }
            }
        }

    }
}
