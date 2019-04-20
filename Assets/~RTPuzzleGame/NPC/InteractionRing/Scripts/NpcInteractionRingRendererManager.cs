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

        public void Tick(ref CommandBuffer cmd)
        {
            if (this.NpcInteractionRingCommandBufferManager != null)
            {
                this.NpcInteractionRingCommandBufferManager.Tick(NpcInteractionRingContainer.ActiveNpcInteractionRings, cmd);
            }
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
        }

        public void Tick(List<NpcInteractionRingType> activeNpcInteractionRings, CommandBuffer cmd)
        {
            if (activeNpcInteractionRings.Count > 0)
            {
                activeNpcInteractionRings.Sort((NpcInteractionRingType r1, NpcInteractionRingType r2) => Vector3.Distance(mainCamera.transform.position, r1.transform.position) >= Vector3.Distance(mainCamera.transform.position, r2.transform.position) ? 0 : 1);
                foreach (var interactionRing in activeNpcInteractionRings)
                {
                    var materialProperty = new MaterialPropertyBlock();
                    materialProperty.SetTexture("_MainTex", interactionRing.RingTexture);
                    cmd.DrawMesh(interactionRing.MeshFilter.mesh, interactionRing.MeshRenderer.localToWorldMatrix, NpcInteractionRingCommandBufferManagerComponent.shaderInteractionRingMaterial, 0, 0, materialProperty);
                }
            }
        }

    }
}
