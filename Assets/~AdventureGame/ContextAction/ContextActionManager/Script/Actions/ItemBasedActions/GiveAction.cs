using CoreGame;
using System;
using System.Collections;
using UnityEngine;
using static CoreGame.PlayerAnimationConstants;

namespace AdventureGame
{

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

        public GiveAction(Item itemGiven, AContextAction nextAction) : base(nextAction)
        {
            this.itemGiven = itemGiven;
            contextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.GIVE_CONTEXT_ACTION_WHEEL_CONFIG;
        }

        public override bool ComputeFinishedConditions()
        {
            return isActionEnded;
        }

        public override void FirstExecutionAction(AContextActionInput ContextActionInput)
        {

            #region External Dependencies
            this.InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
            #endregion
            GiveActionAnimationManager = new GiveActionAnimationManager(itemGiven);

            //reset
            isActionEnded = false;
            itemSuccesfullyGiven = false;
            var giveActionInput = (GiveActionInput)ContextActionInput;

            if (IsIemGivenElligibleToGive(giveActionInput))
            {
                this.itemGiven.StartCoroutine(GiveActionAnimationManager.Start(giveActionInput, () =>
                {
                    this.OnGiveAnimationEnd();
                }));
                itemSuccesfullyGiven = true;
            }
            else
            {
                this.itemGiven.StartCoroutine(AnimationPlayerHelper.Play(giveActionInput.PlayerAnimator, PlayerAnimatioNamesEnum.PLAYER_ACTION_FORBIDDEN, 0f, () =>
                {
                    {
                        this.OnGiveAnimationEnd();
                        return null;
                    }
                }));
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
        private Item ItemGiven;

        public GiveActionAnimationManager(Item itemGiven)
        {
            ItemGiven = itemGiven;
        }

        private Animator PlayerAnimator;
        private GameObject DisplayedItemModel;

        public IEnumerator Start(GiveActionInput giveActionInput, Action onAnimationEndCallback)
        {
            this.PlayerAnimator = giveActionInput.PlayerAnimator;

            PlayerAnimator = giveActionInput.PlayerAnimator;
            return PlayerAnimationPlayer.PlayItemGivenAnimation(giveActionInput.PlayerAnimator,
                onItemShow: InstanciateDisplayedItem,
                onAnimationEnd: () =>
                {
                    HideDisplayedItem();
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
        }

    }
    #endregion
}