using CoreGame;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWheelNode : MonoBehaviour
{
    public const string COOLDWON_TIMER_PATTERN = "ss\\.ff";
    private const string COOLDOWN_OBJECT_NAME = "Cooldown";
    private const string REMAINING_EXECUTION_AMOUNT_OBJECT_NAME = "RemainingExecutionAmount";
    private const string DESCRIPTIOn_TEXT_OBJECT_NAME = "DescriptionText";

    private SelectionWheelNodeCooldownDisplayManager selectionWheelNodeCooldownDisplayManager;
    private SelectionWheelNodeRemainngExecutionAmoutDisaplyManager selectionWheelNodeRemainngExecutionAmoutDisaplyManager;

    #region Internal properties
    private float targetWheelAngleDeg;
    private float currentAngleDeg;
    private Image imageComponent;
    private Text descriptionText;
    #endregion

    #region Specific node data
    private SelectionWheelNodeData wheelNodeData;
    #endregion

    public float TargetWheelAngleDeg { get => targetWheelAngleDeg; set => targetWheelAngleDeg = value; }
    public float CurrentAngleDeg { get => currentAngleDeg; set => currentAngleDeg = value; }
    public Image ImageComponent { get => imageComponent; set => imageComponent = value; }
    public SelectionWheelNodeData WheelNodeData { get => wheelNodeData; set => wheelNodeData = value; }
    internal SelectionWheelNodeCooldownDisplayManager SelectionWheelNodeCooldownDisplayManager { get => selectionWheelNodeCooldownDisplayManager; set => selectionWheelNodeCooldownDisplayManager = value; }

    public static SelectionWheelNode Instantiate(SelectionWheelNodeData wheelNodeData)
    {
        var obj = Instantiate(CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CorePrefabConfiguration.ActionWheelNodePrefab);
        var wheelActionNode = obj.GetComponent<SelectionWheelNode>();
        wheelActionNode.imageComponent = obj.GetComponent<Image>();
        wheelActionNode.descriptionText = obj.gameObject.FindChildObjectRecursively(DESCRIPTIOn_TEXT_OBJECT_NAME).GetComponent<Text>();
        wheelActionNode.descriptionText.text = wheelNodeData.NodeText;

        wheelActionNode.imageComponent.sprite = wheelNodeData.NodeSprite;
        wheelActionNode.WheelNodeData = wheelNodeData;

        wheelActionNode.selectionWheelNodeCooldownDisplayManager = new SelectionWheelNodeCooldownDisplayManager(wheelActionNode.gameObject.FindChildObjectRecursively(COOLDOWN_OBJECT_NAME), wheelActionNode);
        wheelActionNode.selectionWheelNodeRemainngExecutionAmoutDisaplyManager = new SelectionWheelNodeRemainngExecutionAmoutDisaplyManager(wheelActionNode.gameObject.FindChildObjectRecursively(REMAINING_EXECUTION_AMOUNT_OBJECT_NAME), wheelActionNode);

        return wheelActionNode;
    }

    public void Tick(float d)
    {
        this.selectionWheelNodeCooldownDisplayManager.Tick();
        this.selectionWheelNodeRemainngExecutionAmoutDisaplyManager.Tick();
    }

    public void SetMaterial(Material material)
    {
        imageComponent.material = material;
    }

    public void SetActiveText(bool value)
    {
        this.descriptionText.gameObject.SetActive(value);
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
        }

        if (!this.wheelNodeRef.WheelNodeData.CanNodeBeExecuted)
        {
            nodeImage.color = nodeDarkerImageColor;
        }
    }
}

class SelectionWheelNodeRemainngExecutionAmoutDisaplyManager
{
    private GameObject remainingAmountObject;
    private Text remainingAmountText;
    private Outline remainingAmountOutlineText;
    private SelectionWheelNode wheelNodeRef;

    private Color nodeInitialOutlineColor;
    private Color nodeDarkerOutlineColor;

    public SelectionWheelNodeRemainngExecutionAmoutDisaplyManager(GameObject remainingAmountObject, SelectionWheelNode wheelNodeRef)
    {
        this.wheelNodeRef = wheelNodeRef;
        this.remainingAmountObject = remainingAmountObject;
        this.remainingAmountText = remainingAmountObject.GetComponent<Text>();
        this.remainingAmountOutlineText = remainingAmountText.GetComponent<Outline>();

        this.nodeInitialOutlineColor = this.remainingAmountOutlineText.effectColor;
        float darkenColorFactor = 0.8f;
        this.nodeDarkerOutlineColor = new Color(this.nodeInitialOutlineColor.r * darkenColorFactor, this.nodeInitialOutlineColor.g * darkenColorFactor, this.nodeInitialOutlineColor.b * darkenColorFactor);

        if (wheelNodeRef.WheelNodeData.GetRemainingExecutionAmount == -1)
        {
            this.remainingAmountText.text = '\u221E'.ToString();
            this.remainingAmountObject.SetActive(true);
        }
        else if (wheelNodeRef.WheelNodeData.GetRemainingExecutionAmount == -2)
        {
            this.remainingAmountText.text = "";
        }
        else
        {
            this.remainingAmountText.text = wheelNodeRef.WheelNodeData.GetRemainingExecutionAmount.ToString();
            this.remainingAmountObject.SetActive(true);
        }
    }

    public void Tick()
    {
        this.remainingAmountOutlineText.effectColor = this.nodeInitialOutlineColor;
        if (!this.wheelNodeRef.WheelNodeData.CanNodeBeExecuted)
        {
            this.remainingAmountOutlineText.effectColor = this.nodeDarkerOutlineColor;
        }
    }
}

public abstract class SelectionWheelNodeData
{
    public abstract object Data { get; }
    public abstract Sprite NodeSprite { get; }
    public abstract bool CanNodeBeExecuted { get; }

    public abstract string NodeText { get; }

    #region Cooldown management
    public abstract bool IsOnCoolDown { get; }
    public abstract float GetRemainingCooldownTime { get; }
    #endregion

    #region
    public abstract int GetRemainingExecutionAmount { get; }
    #endregion
}