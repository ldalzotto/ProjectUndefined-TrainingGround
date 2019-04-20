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

        private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        private GroundEffectsManager GroundEffectsManager;

        public RangeFXPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
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

                    var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                    var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);

                    var activeFXMaterials = this.GroundEffectsManager.GetActiveFXMaterials();
                    if (activeFXMaterials != null)
                    {
                        foreach (var effectMaterial in activeFXMaterials)
                        {
                            if (effectMaterial != null)
                            {
                                drawSettings.overrideMaterial = effectMaterial;
                                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);
                            }
                        }
                    }
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }

}
