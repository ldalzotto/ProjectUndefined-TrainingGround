using UnityEngine;
using System.Collections;
using RTPuzzle;

namespace UnityEngine.Rendering.LWRP
{
    public class InteractionRingPass : ScriptableRenderPass
    {
        const string m_ProfilerTag = "InteractionRing";

        public InteractionRingPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
        }

        private NpcInteractionRingRendererManager npcInteractionRingRendererManager;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (this.npcInteractionRingRendererManager == null)
            {
                this.npcInteractionRingRendererManager = GameObject.FindObjectOfType<NpcInteractionRingRendererManager>();
            }

            if (this.npcInteractionRingRendererManager != null)
            {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                using (new ProfilingSample(cmd, m_ProfilerTag))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    this.npcInteractionRingRendererManager.Tick(ref cmd);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }

}
