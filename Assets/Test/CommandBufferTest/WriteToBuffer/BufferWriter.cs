using UnityEngine;
using UnityEngine.Rendering;

public class BufferWriter : MonoBehaviour
{
    public Camera Camera;

    public MeshRenderer meshRend;
    public Material material;

    public bool update;
    public Color setColor;

    private void Start()
    {
        BuildCommandBuffer();
    }

    private void Update()
    {
        material.SetColor("_NewCol", setColor);
    }

    private void BuildCommandBuffer()
    {
        var command = new CommandBuffer();
        command.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
        command.DrawRenderer(meshRend, material);
        Camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, command);
    }

}
