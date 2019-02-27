using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWheelNode : MonoBehaviour
{
    public const string COOLDWON_TIMER_PATTERN = "ss\\.ff";
    private const string COOLDOWN_OBJECT_NAME = "Cooldown";

    private SelectionWheelNodeCooldownDisplayManager selectionWheelNodeCooldownDisplayManager;

    #region Internal properties
    private float targetWheelAngleDeg;
    private float currentAngleDeg;
    private Image imageComponent;
    #endregion

    #region Specific node data
    private SelectionWheelNodeData wheelNodeData;
    #endregion

    public float TargetWheelAngleDeg { get => targetWheelAngleDeg; set => targetWheelAngleDeg = value; }
    public float CurrentAngleDeg { get => currentAngleDeg; set => currentAngleDeg = value; }
    public Image ImageComponent { get => imageComponent; set => imageComponent = value; }
    public SelectionWheelNodeData WheelNodeData { get => wheelNodeData; set => wheelNodeData = value; }
    internal SelectionWheelNodeCooldownDisplayManager SelectionWheelNodeCooldownDisplayManager { get => selectionWheelNodeCooldownDisplayManager; set => selectionWheelNodeCooldownDisplayManager = value; }

    public static SelectionWheelNode Instantiate(SelectionWheelNodeData wheelNodeData, WheelNodeSpriteResolver NodeSpriteResolver)
    {
        var obj = Instantiate(PrefabContainer.Instance.ActionWheelNodePrefab);
        var wheelActionNode = obj.GetComponent<SelectionWheelNode>();
        wheelActionNode.imageComponent = obj.GetComponent<Image>();

        wheelActionNode.imageComponent.sprite = NodeSpriteResolver.Invoke(wheelNodeData);
        wheelActionNode.WheelNodeData = wheelNodeData;

        wheelActionNode.selectionWheelNodeCooldownDisplayManager = new SelectionWheelNodeCooldownDisplayManager(wheelActionNode.gameObject.FindChildObjectRecursively(COOLDOWN_OBJECT_NAME), wheelActionNode);

        return wheelActionNode;
    }

    public void Tick(float d)
    {
        selectionWheelNodeCooldownDisplayManager.Tick();
    }

    public void SetMaterial(Material material)
    {
        imageComponent.material = material;
    }
}


class SelectionWheelNodeCooldownDisplayManager
{
    private SelectionWheelNode wheelNodeRef;
    private GameObject cooldownObject;
    private Text cooldownText;
    private Image nodeImage;

    private Color nodeInitialImageColor;
    private Color nodeDarkerImageColor;

    public SelectionWheelNodeCooldownDisplayManager(GameObject cooldownObject, SelectionWheelNode wheelNodeRef)
    {
        this.cooldownObject = cooldownObject;
        this.cooldownText = cooldownObject.GetComponent<Text>();
        this.wheelNodeRef = wheelNodeRef;
        this.nodeImage = wheelNodeRef.ImageComponent;
        this.nodeInitialImageColor = nodeImage.color;
        float darkenColorFactor = 0.8f;
        this.nodeDarkerImageColor = new Color(this.nodeInitialImageColor.r * darkenColorFactor, this.nodeInitialImageColor.g * darkenColorFactor, this.nodeInitialImageColor.b * darkenColorFactor);
    }

    public void Tick()
    {
        bool isOnCooldown = this.wheelNodeRef.WheelNodeData.IsOnCoolDown;
        cooldownObject.SetActive(isOnCooldown);
        nodeImage.color = nodeInitialImageColor;
        if (isOnCooldown)
        {
            this.cooldownText.text = TimeSpan.FromSeconds(this.wheelNodeRef.WheelNodeData.GetRemainingCooldownTime).ToString(SelectionWheelNode.COOLDWON_TIMER_PATTERN);
            nodeImage.color = nodeDarkerImageColor;
        }
    }
}


public delegate Sprite WheelNodeSpriteResolver(SelectionWheelNodeData SelectionWheelNodeData);

public abstract class SelectionWheelNodeData
{
    public abstract object Data { get; }

    #region Cooldown management
    public abstract bool IsOnCoolDown { get; }
    public abstract float GetRemainingCooldownTime { get; }
    #endregion
}