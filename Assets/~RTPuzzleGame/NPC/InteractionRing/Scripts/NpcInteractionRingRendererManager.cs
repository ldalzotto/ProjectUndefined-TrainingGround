using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class NpcInteractionRingRendererManager : MonoBehaviour
    {

        public NpcInteractionRingCommandBufferManagerComponent NpcInteractionRingCommandBufferManagerComponent;

        private NpcInteractionRingCommandBufferManager NpcInteractionRingCommandBufferManager;

        #region External Dependencies
        private NpcInteractionRingContainer NpcInteractionRingContainer;
        #endregion

        public void Init()
        {
            this.NpcInteractionRingCommandBufferManager = new NpcInteractionRingCommandBufferManager(NpcInteractionRingCommandBufferManagerComponent, Camera.main);
            this.NpcInteractionRingContainer = GameObject.FindObjectOfType<NpcInteractionRingContainer>();
        }

        public void Tick(float d)
        {
            this.NpcInteractionRingCommandBufferManager.Tick(d, NpcInteractionRingContainer.ActiveNpcInteractionRings);
        }

    }

    [System.Serializable]
    public class NpcInteractionRingCommandBufferManagerComponent
    {
        public Material shaderInteractionRingMaterial;
    }

    class NpcInteractionRingCommandBufferManager
    {
        private NpcInteractionRingCommandBufferManagerComponent NpcInteractionRingCommandBufferManagerComponent;
        private Camera mainCamera;

        public NpcInteractionRingCommandBufferManager(NpcInteractionRingCommandBufferManagerComponent NpcInteractionRingCommandBufferManagerComponent, Camera mainCamera)
        {
            this.NpcInteractionRingCommandBufferManagerComponent = NpcInteractionRingCommandBufferManagerComponent;
            this.mainCamera = mainCamera;
            this.interactionRingBuffer = new CommandBuffer();
            this.interactionRingBuffer.name = "Interaction Ring Render";

            this.mainCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.interactionRingBuffer);
        }

        private CommandBuffer interactionRingBuffer;

        public void Tick(float d, HashSet<NpcInteractionRingType> activeNpcInteractionRings)
        {
            this.interactionRingBuffer.Clear();
            if (activeNpcInteractionRings.Count > 0)
            {
                var materialProperty = new MaterialPropertyBlock();
                foreach (var interactionRing in activeNpcInteractionRings)
                {
                    materialProperty.SetTexture("_MainTex", interactionRing.RingTexture);
                    this.interactionRingBuffer.DrawMesh(interactionRing.MeshFilter.mesh, interactionRing.MeshRenderer.localToWorldMatrix, NpcInteractionRingCommandBufferManagerComponent.shaderInteractionRingMaterial, 0, 0, materialProperty);
                }
            }
        }

    }
}
