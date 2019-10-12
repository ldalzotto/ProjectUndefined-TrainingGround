using CoreGame;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

namespace RTPuzzle
{
    [CreateAssetMenu(fileName = "PuzzleSelectionObjectRederingPassFeature", menuName = "Rendering/PuzzleSelectionObjectRederingPassFeature")]
    public class PuzzleSelectionObjectRederingPassFeature : ScriptableRendererFeature
    {
        class CustomRenderPass : ScriptableRenderPass
        {
            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {

                var levelManager = CoreGameSingletonInstances.LevelManager;
                if (levelManager != null && levelManager.CurrentLevelType == LevelType.PUZZLE)
                {
                    var interactiveObjectSelectionManager = InteractiveObjectSelectionManager.Get();
                    if (interactiveObjectSelectionManager.InteractiveObjectSelectionRendererManager != null
                         && interactiveObjectSelectionManager.InteractiveObjectSelectionRendererManager.CommandBufer != null)
                    {
                        context.ExecuteCommandBuffer(interactiveObjectSelectionManager.InteractiveObjectSelectionRendererManager.CommandBufer);
                    }
                }

            }
        }

        CustomRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new CustomRenderPass();
            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }



}
