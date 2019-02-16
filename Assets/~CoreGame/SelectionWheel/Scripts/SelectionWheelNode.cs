using UnityEngine;
using UnityEngine.UI;

public class SelectionWheelNode : MonoBehaviour
{

    private float targetWheelAngleDeg;
    private float currentAngleDeg;

    private SelectionWheelNodeData wheelNodeData;

    private Image imageComponent;


    public float TargetWheelAngleDeg { get => targetWheelAngleDeg; set => targetWheelAngleDeg = value; }
    public float CurrentAngleDeg { get => currentAngleDeg; set => currentAngleDeg = value; }
    public Image ImageComponent { get => imageComponent; set => imageComponent = value; }
    public SelectionWheelNodeData WheelNodeData { get => wheelNodeData; set => wheelNodeData = value; }

    public static SelectionWheelNode Instantiate(SelectionWheelNodeData wheelNodeData, WheelNodeSpriteResolver NodeSpriteResolver)
    {
        var obj = Instantiate(PrefabContainer.Instance.ActionWheelNodePrefab);
        var wheelActionNode = obj.GetComponent<SelectionWheelNode>();
        wheelActionNode.imageComponent = obj.GetComponent<Image>();

        wheelActionNode.imageComponent.sprite = NodeSpriteResolver.Invoke(wheelNodeData);
        wheelActionNode.WheelNodeData = wheelNodeData;
        return wheelActionNode;
    }

    public void SetMaterial(Material material)
    {
        imageComponent.material = material;
    }
}

public delegate Sprite WheelNodeSpriteResolver(SelectionWheelNodeData SelectionWheelNodeData);

public abstract class SelectionWheelNodeData
{
    public abstract object Data { get; }
}