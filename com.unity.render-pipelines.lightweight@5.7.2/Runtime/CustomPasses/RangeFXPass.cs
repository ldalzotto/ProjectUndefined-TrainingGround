using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTPuzzle;

namespace UnityEngine.Rendering.LWRP
{
    internal class RangeFXPass : ScriptableRenderPass
    {
        FilteringSettings m_FilteringSettings;
        const string m_ProfilerTag = "Range FX";

        private Material rangeFXMaterial;

        private GroundEffectsManager GroundEffectsManager;

        public RangeFXPass(RenderPassEvent evt, Material rangeFXMaterial, RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            this.rangeFXMaterial = rangeFXMaterial;
            renderPassEvent = evt;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (this.GroundEffectsManager == null)
            {
                this.GroundEffectsManager = GameObject.FindObjectOfType<GroundEffectsManager>();
            }

            if (this.GroundEffectsManager != null)
            {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                using (new ProfilingSample(cmd, m_ProfilerTag))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    GroundEffectsManager.OnCommandBufferUpdate(cmd);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }

}
