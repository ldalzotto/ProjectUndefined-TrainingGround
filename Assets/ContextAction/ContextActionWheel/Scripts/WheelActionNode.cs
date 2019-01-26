using UnityEngine;
using UnityEngine.UI;

public class WheelActionNode : MonoBehaviour
{

    private float targetWheelAngleDeg;
    private float currentAngleDeg;

    private AContextAction associatedContextAction;

    private Image imageComponent;

    public float TargetWheelAngleDeg { get => targetWheelAngleDeg; set => targetWheelAngleDeg = value; }
    public float CurrentAngleDeg { get => currentAngleDeg; set => currentAngleDeg = value; }
    public AContextAction AssociatedContextAction { get => associatedContextAction; set => associatedContextAction = value; }
    public Image ImageComponent { get => imageComponent; set => imageComponent = value; }

    public static WheelActionNode Instantiate(AContextAction contextAction)
    {
        var obj = Instantiate(PrefabContainer.Instance.ActionWheelNodePrefab);
        var wheelActionNode = obj.GetComponent<WheelActionNode>();
        wheelActionNode.imageComponent = obj.GetComponent<Image>();

        wheelActionNode.imageComponent.sprite = ContextActionWheelNodeConfiguration.contextActionWheelNodeConfiguration[contextAction.ContextActionWheelNodeConfigurationId].ContextActionWheelIcon;

        wheelActionNode.AssociatedContextAction = contextAction;
        return wheelActionNode;
    }

    public void SetMaterial(Material material)
    {
        imageComponent.material = material;
    }
}
