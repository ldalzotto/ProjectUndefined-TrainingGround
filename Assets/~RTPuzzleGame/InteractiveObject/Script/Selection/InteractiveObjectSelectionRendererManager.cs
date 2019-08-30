using CoreGame;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class InteractiveObjectSelectionRendererManager
    {
        private ComputeShader OutlineComputeShader;
        private Material OutlineColorShader;
        private Material BufferScreenSampleShader;

        private CommandBuffer commandBufer;

        #region Internal Dependencies
        private InteractiveObjectSelectionManager InteractiveObjectSelectionManager;
        #endregion

        public InteractiveObjectSelectionRendererManager(InteractiveObjectSelectionManager InteractiveObjectSelectionManager, ComputeShader OutlineComputeShader, Material OutlineColorShader, Material BufferScreenSampleShader)
        {
            this.InteractiveObjectSelectionManager = InteractiveObjectSelectionManager;
            this.OutlineComputeShader = OutlineComputeShader;
            this.OutlineColorShader = OutlineColorShader;
            this.BufferScreenSampleShader = BufferScreenSampleShader;

            this.commandBufer = new CommandBuffer();
            this.commandBufer.name = this.GetType().Name;

            Camera.main.AddCommandBuffer(CameraEvent.AfterForwardAlpha, this.commandBufer);
        }

        public void Tick(float d)
        {
            this.commandBufer.Clear();

            if (this.InteractiveObjectSelectionManager.GetCurrentSelectedObject() != null)
            {
                var modelOutlined = this.InteractiveObjectSelectionManager.GetCurrentSelectedObject().ModelObjectModule;

                if (modelOutlined != null)
                {
                    int width = Camera.main.pixelWidth;
                    int height = Camera.main.pixelHeight;

                    var rangeRenderBuffer = Shader.PropertyToID(OutlineComputeShaderConstants.OUTLINE_TARGET_TEXTURE);
                    this.commandBufer.GetTemporaryRT(rangeRenderBuffer, new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB64) { sRGB = false, autoGenerateMips = false, enableRandomWrite = true });
                    this.commandBufer.SetRenderTarget(rangeRenderBuffer);
                    this.commandBufer.ClearRenderTarget(true, true, MyColors.TransparentBlack);

                    var tmpRangeRenderArrayBuffer = Shader.PropertyToID(OutlineComputeShaderConstants.PRE_EFFECT_TEXTURES);
                    this.commandBufer.GetTemporaryRTArray(tmpRangeRenderArrayBuffer, width, height, 1, 0, FilterMode.Point, RenderTextureFormat.ARGB64, RenderTextureReadWrite.Linear, 1, true);

                    this.commandBufer.SetRenderTarget(new RenderTargetIdentifier(tmpRangeRenderArrayBuffer, depthSlice: 0));
                    this.commandBufer.ClearRenderTarget(true, true, MyColors.TransparentBlack);
                    
                    foreach (var meshRenderer in modelOutlined.MeshRenderers)
                    {
                        this.commandBufer.DrawRenderer(meshRenderer, this.OutlineColorShader);
                    }

                    this.commandBufer.SetComputeTextureParam(this.OutlineComputeShader, this.OutlineComputeShader.FindKernel(OutlineComputeShaderConstants.OUTLINE_KERNEL), rangeRenderBuffer, new RenderTargetIdentifier(rangeRenderBuffer));
                    this.commandBufer.SetComputeTextureParam(this.OutlineComputeShader, this.OutlineComputeShader.FindKernel(OutlineComputeShaderConstants.OUTLINE_KERNEL), tmpRangeRenderArrayBuffer, new RenderTargetIdentifier(tmpRangeRenderArrayBuffer));

                    this.commandBufer.SetComputeIntParam(this.OutlineComputeShader, OutlineComputeShaderConstants.TEXTURE_WIDTH, width);
                    this.commandBufer.SetComputeIntParam(this.OutlineComputeShader, OutlineComputeShaderConstants.TEXTURE_HEIGHT, height);

                    this.commandBufer.DispatchCompute(this.OutlineComputeShader, this.OutlineComputeShader.FindKernel(OutlineComputeShaderConstants.OUTLINE_KERNEL), width / 8, height / 8, 1);

                    this.commandBufer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);

                    foreach (var meshRenderer in modelOutlined.MeshRenderers)
                    {
                        this.commandBufer.DrawRenderer(meshRenderer, this.BufferScreenSampleShader);
                    }

                    this.commandBufer.ReleaseTemporaryRT(rangeRenderBuffer);
                    this.commandBufer.ReleaseTemporaryRT(tmpRangeRenderArrayBuffer);
                }
            }
        }
    }
}
