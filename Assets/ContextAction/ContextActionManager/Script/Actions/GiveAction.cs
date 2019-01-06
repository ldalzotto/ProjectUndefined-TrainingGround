using System;
using System.Collections;
using UnityEngine;

public class GiveAction : AContextAction
{
    private Item itemGiven;
    private bool isActionEnded;

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
    private void OnAnimationEnd()
    {
        isActionEnded = true;
    }
    #endregion

    public override void OnStart()
    {
        #region External Dependencies
        var PlayerGlobalAnimationEventHandler = GameObject.FindObjectOfType<PlayerGlobalAnimationEventHandler>();
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
        var giveActionInput = (GiveActionInput)ContextActionInput;

        if (IsIemGivenElligibleToGive(giveActionInput))
        {
            StartCoroutine(GiveActionAnimationManager.Start(giveActionInput, OnAnimationEnd));
        }
        else
        {
            StartCoroutine(AnimationPlayerHelper.Play(giveActionInput.PlayerAnimator, PlayerAnimatioNnamesEnum.PLAYER_ACTION_FORBIDDEN, 0f, OnAnimationEnd));
        }
    }

    public override void Tick(float d)
    {

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
        //TODO process to give
        //TODO animation
        PlayerGlobalAnimationEventHandler.OnShowGivenItem += InstanciateDisplayedItem;
        PlayerAnimator = giveActionInput.PlayerAnimator;
        return AnimationPlayerHelper.Play(giveActionInput.PlayerAnimator, PlayerAnimatioNnamesEnum.PLAYER_ACTION_GIVE_OBJECT, 0f, () =>
         {
             Debug.Log("Rotate Object");
             MonoBehaviour.Destroy(DisplayedItemModel);
             PlayerGlobalAnimationEventHandler.OnShowGivenItem -= InstanciateDisplayedItem;
             onAnimationEndCallback.Invoke();
         });

    }

    private void InstanciateDisplayedItem()
    {
        if (PlayerAnimator != null)
        {
            var rightHandBoneTransform = PlayerAnimator.gameObject.FindChildObjectRecursively(AnimationConstants.RIGHT_HAND_PLAYER_BONE_NAME).transform;
            DisplayedItemModel = MonoBehaviour.Instantiate(ItemGiven.ItemModel, rightHandBoneTransform, false);
            var scaleFactor = Vector3.one;
            ComponentSearchHelper.ComputeScaleFactorRecursively(rightHandBoneTransform, PlayerAnimator.transform, ref scaleFactor);
            DisplayedItemModel.transform.localScale = new Vector3(
                DisplayedItemModel.transform.localScale.x / scaleFactor.x,
                DisplayedItemModel.transform.localScale.y / scaleFactor.y,
                DisplayedItemModel.transform.localScale.z / scaleFactor.z);
        }
    }

}
#endregion