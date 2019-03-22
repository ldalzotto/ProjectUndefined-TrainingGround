using UnityEngine;
using UnityEngine.Rendering;

public class CommandBufferTransTest : MonoBehaviour
{
    public Camera camera;
    public Material targetMateial;
    public GameObject targetObject;

    private CommandBuffer commandBuffer;

    private void Start()
    {
        this.commandBuffer = new CommandBuffer();
        this.commandBuffer.name = "Target";
        this.commandBuffer.Clear();
        this.commandBuffer.DrawRenderer(this.targetObject.GetComponent<MeshRenderer>(), targetMateial, 0, 0);
        this.commandBuffer.DrawRenderer(this.targetObject.GetComponent<MeshRenderer>(), targetMateial, 0, 1);
        this.camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
  
    }

}
