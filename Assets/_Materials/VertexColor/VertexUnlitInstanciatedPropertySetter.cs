using UnityEngine;

public class VertexUnlitInstanciatedPropertySetter : MonoBehaviour
{

    public Color colorToSet = Color.white;
    public bool everyFrame;

    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propertyBlock;

    public void Init()
    {
        this.meshRenderer = GetComponent<MeshRenderer>();
        this.propertyBlock = new MaterialPropertyBlock();
        this.propertyBlock.SetColor("_Color", colorToSet);
        this.meshRenderer.SetPropertyBlock(this.propertyBlock);
    }

    public void Tick(float d)
    {
        if (everyFrame)
        {
            this.propertyBlock.SetColor("_Color", colorToSet);
            this.meshRenderer.SetPropertyBlock(this.propertyBlock);
        }
    }
}
