using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class RangeEffectRework : MonoBehaviour
{
    public List<MeshRenderer> MeshesToRender;
    public Material WorldPositionBufferShader;
    public Material SphereRangeShader;
    public Material RangeEdgeImageEffectShader;
    public Material RangeToCameraShader;

    private CommandBuffer worldPositionBufferCommand;
    private CommandBuffer rangeDrawCommand;
    private CommandBuffer releaseCommand;

    void Start()
    {
        var cam = GetComponent<Camera>();
        cam.depth = 0;

        this.worldPositionBufferCommand = new CommandBuffer();
        this.worldPositionBufferCommand.name = nameof(this.worldPositionBufferCommand);
        this.releaseCommand = new CommandBuffer();
        this.releaseCommand.name = nameof(this.releaseCommand);
        this.rangeDrawCommand = new CommandBuffer();
        this.rangeDrawCommand.name = nameof(this.rangeDrawCommand);
        cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.worldPositionBufferCommand);
        cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.rangeDrawCommand);
        cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.releaseCommand);
    }

    void Update()
    {
        // this.MeshesToRender =  this.MeshesToRender.OrderBy(a => Guid.NewGuid()).ToList();
        var meshes = this.MeshesToRender.ToList().Select(r => r).Where(r => r.isVisible).ToList();

        this.worldPositionBufferCommand.Clear();
        this.releaseCommand.Clear();
        this.rangeDrawCommand.Clear();

        var worldPositionBuffer = Shader.PropertyToID("_WorldPositionBuffer");
        this.worldPositionBufferCommand.GetTemporaryRT(worldPositionBuffer, new RenderTextureDescriptor(Camera.main.pixelWidth, Camera.main.pixelHeight, RenderTextureFormat.ARGBFloat, 16) { sRGB = false, autoGenerateMips = false });
        this.worldPositionBufferCommand.SetGlobalTexture(worldPositionBuffer, new RenderTargetIdentifier(worldPositionBuffer));
        this.worldPositionBufferCommand.SetRenderTarget(new RenderTargetIdentifier(worldPositionBuffer));
        this.worldPositionBufferCommand.ClearRenderTarget(true, true, Color.black);

        foreach (var meshToRender in meshes)
        {
            this.worldPositionBufferCommand.DrawRenderer(meshToRender, this.WorldPositionBufferShader);
        }

        var rangeRenderBuffer = Shader.PropertyToID("_RangeRenderBuffer");
        this.rangeDrawCommand.GetTemporaryRT(rangeRenderBuffer, new RenderTextureDescriptor(Camera.main.pixelWidth, Camera.main.pixelHeight, RenderTextureFormat.ARGBFloat) { sRGB = false, autoGenerateMips = false });

        var tmpRangeRenderBuffer = Shader.PropertyToID("_TmpRangeRenderBuffer");
        this.rangeDrawCommand.GetTemporaryRT(tmpRangeRenderBuffer, new RenderTextureDescriptor(Camera.main.pixelWidth, Camera.main.pixelHeight, RenderTextureFormat.ARGBFloat) { sRGB = false, autoGenerateMips = false });
        // this.rangeDrawCommand.SetRenderTarget(tmpRangeRenderBuffer);
        // this.rangeDrawCommand.ClearRenderTarget(true, true, Color.black);
        this.rangeDrawCommand.Blit(new RenderTargetIdentifier(worldPositionBuffer), new RenderTargetIdentifier(tmpRangeRenderBuffer), this.SphereRangeShader);
        this.rangeDrawCommand.Blit(new RenderTargetIdentifier(tmpRangeRenderBuffer), new RenderTargetIdentifier(rangeRenderBuffer), this.RangeEdgeImageEffectShader);
        this.rangeDrawCommand.SetGlobalTexture(tmpRangeRenderBuffer, new RenderTargetIdentifier(tmpRangeRenderBuffer));

        this.rangeDrawCommand.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
        foreach (var meshToRender in meshes)
        {
            this.rangeDrawCommand.DrawRenderer(meshToRender, this.RangeToCameraShader);
        }


        this.releaseCommand.ReleaseTemporaryRT(worldPositionBuffer);
        this.releaseCommand.ReleaseTemporaryRT(rangeRenderBuffer);
        this.releaseCommand.ReleaseTemporaryRT(tmpRangeRenderBuffer);
    }
}
