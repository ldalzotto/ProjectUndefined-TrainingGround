using System;
using System.Collections;
using UnityEngine;

public class GiveAction : AContextAction
{
    private Item itemGiven;
    private bool isActionEnded;
    private bool itemSuccesfullyGiven;

    public Item ItemGiven { get => itemGiven; }

    #region External Dependencies
    private InventoryEventManager InventoryEventManager;
    #endregion

    #region Internal Managers
    private GiveActionAnimationManager GiveActionAnimationManager;
    #endregion

    #region Logical Conditions
    private bool IsIemGivenElligibleToGive(GiveActionInput giveActionInput)
    {
        return (giveActionInput != null && giveActionInput.TargetPOI != null && giveActionInput.TargetPOI.IsElligibleToGiveItem(itemGiven));
    }
    #endregion

    #region Internal Events
    private void OnGiveAnimationEnd()
    {
        isActionEnded = true;
    }
    #endregion

    public override void OnStart()
    {
        #region External Dependencies
        var PlayerGlobalAnimationEventHandler = GameObject.FindObjectOfType<PlayerGlobalAnimationEventHandler>();
        this.InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        #endregion

        itemGiven = GetComponentInParent<Item>();
        GiveActionAnimationManager = new GiveActionAnimationManager(PlayerGlobalAnimationEventHandler, itemGiven);
    }

    public override bool ComputeFinishedConditions()
    {
        return isActionEnded;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        isActionEnded = false;
        itemSuccesfullyGiven = false;
        var giveActionInput = (GiveActionInput)ContextActionInput;

        if (IsIemGivenElligibleToGive(giveActionInput))
        {
            StartCoroutine(GiveActionAnimationManager.Start(giveActionInput, OnGiveAnimationEnd));
            itemSuccesfullyGiven = true;
        }
        else
        {
            StartCoroutine(AnimationPlayerHelper.Play(giveActionInput.PlayerAnimator, PlayerAnimatioNnamesEnum.PLAYER_ACTION_FORBIDDEN, 0f, OnGiveAnimationEnd));
        }
    }

    public override void AfterFinishedEventProcessed()
    {
        if (itemSuccesfullyGiven)
        {
            InventoryEventManager.OnItemGiven(itemGiven);
        }
    }
    public override void Tick(float d)
    {
        GiveActionAnimationManager.Tick(d);
    }

}

#region Action Input
public class GiveActionInput : AContextActionInput
{
    private PointOfInterestType targetPOI;
    private Animator playerAnimator;

    public GiveActionInput(PointOfInterestType targetPOI, Animator playerAnimator)
    {
        this.targetPOI = targetPOI;
        this.playerAnimator = playerAnimator;
    }

    public PointOfInterestType TargetPOI { get => targetPOI; }
    public Animator PlayerAnimator { get => playerAnimator; }
}
#endregion

#region Give Action Animation
class GiveActionAnimationManager
{

    private PlayerGlobalAnimationEventHandler PlayerGlobalAnimationEventHandler;
    private Item ItemGiven;

    public GiveActionAnimationManager(PlayerGlobalAnimationEventHandler playerGlobalAnimationEventHandler, Item itemGiven)
    {
        PlayerGlobalAnimationEventHandler = playerGlobalAnimationEventHandler;
        ItemGiven = itemGiven;
    }

    private Animator PlayerAnimator;
    private GameObject DisplayedItemModel;

    public IEnumerator Start(GiveActionInput giveActionInput, Action onAnimationEndCallback)
    {
        this.PlayerAnimator = giveActionInput.PlayerAnimator;

        PlayerGlobalAnimationEventHandler.OnShowGivenItem += InstanciateDisplayedItem;
        PlayerGlobalAnimationEventHandler.OnHideGivenItem += HideDisplayedItem;

        PlayerAnimator = giveActionInput.PlayerAnimator;
        return AnimationPlayerHelper.Play(giveActionInput.PlayerAnimator, PlayerAnimatioNnamesEnum.PLAYER_ACTION_GIVE_OBJECT, 0f, () =>
         {
             PlayerGlobalAnimationEventHandler.OnShowGivenItem -= InstanciateDisplayedItem;
             PlayerGlobalAnimationEventHandler.OnHideGivenItem -= HideDisplayedItem;
             onAnimationEndCallback.Invoke();
         });
    }

    private void InstanciateDisplayedItem()
    {
        if (PlayerAnimator != null)
        {
            var rightHandBoneTransform = PlayerBoneRetriever.GetPlayerBone(PlayerBone.RIGHT_HAND_CONTEXT, PlayerAnimator).transform;
            var scaleFactor = Vector3.one;
            ComponentSearchHelper.ComputeScaleFactorRecursively(rightHandBoneTransform, PlayerAnimator.transform, ref scaleFactor);
            DisplayedItemModel = GiveActionMiniatureInstanciate.Instance(ItemGiven.ItemModel, rightHandBoneTransform, scaleFactor);
        }
    }

    private void HideDisplayedItem()
    {
        MonoBehaviour.Destroy(DisplayedItemModel);
    }

    public void Tick(float d)
    {
        /**
        if (DisplayedItemModel != null)
        {
            DisplayedItemModel.transform.Rotate(Vector3.up * 180f * d, Space.World);
        }**/

    }

}
#endregion