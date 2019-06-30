using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;
using static AnimationConstants;

namespace AdventureGame
{
    [System.Serializable]
    public class GiveAction : AContextAction
    {
        [SerializeField]
        private ItemID itemGiven;
        [NonSerialized]
        private bool isActionEnded;
        [NonSerialized]
        private bool itemSuccesfullyGiven;

        public ItemID ItemGiven { get => itemGiven; }

        #region External Dependencies
        [NonSerialized]
        private InventoryEventManager InventoryEventManager;
        #endregion

        #region Internal Managers
        [NonSerialized]
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

        public GiveAction(ItemID itemGiven, AContextAction nextAction) : base(nextAction)
        {
            this.itemGiven = itemGiven;
            contextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.GIVE_CONTEXT_ACTION_WHEEL_CONFIG;
        }

        public override bool ComputeFinishedConditions()
        {
            return isActionEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {

            #region External Dependencies
            this.InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
            var AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            var animationConfiguration = GameObject.FindObjectOfType<CoreConfigurationManager>().AnimationConfiguration();
            #endregion
            GiveActionAnimationManager = new GiveActionAnimationManager(itemGiven, AdventureGameConfigurationManager, animationConfiguration);

            //reset
            isActionEnded = false;
            itemSuccesfullyGiven = false;
            var giveActionInput = (GiveActionInput)ContextActionInput;

            if (IsIemGivenElligibleToGive(giveActionInput))
            {
                Coroutiner.Instance.StartCoroutine(GiveActionAnimationManager.Start(giveActionInput, () =>
                {
                    this.OnGiveAnimationEnd();
                }));
                itemSuccesfullyGiven = true;
            }
            else
            {
                Coroutiner.Instance.StartCoroutine(AnimationPlayerHelper.PlayAndWait(giveActionInput.PlayerAnimator, animationConfiguration.ConfigurationInherentData[AnimationID.ACTION_FORBIDDEN], 0f, () =>
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
        private ItemID ItemGiven;
        private AdventureGameConfigurationManager AdventureGameConfigurationManager;
        private AnimationConfiguration AnimationConfiguration;

        public GiveActionAnimationManager(ItemID itemGiven, AdventureGameConfigurationManager AdventureGameConfigurationManager, AnimationConfiguration AnimationConfiguration)
        {
            ItemGiven = itemGiven;
            this.AdventureGameConfigurationManager = AdventureGameConfigurationManager;
            this.AnimationConfiguration = AnimationConfiguration;
        }

        private Animator PlayerAnimator;
        private GameObject DisplayedItemModel;

        public IEnumerator Start(GiveActionInput giveActionInput, Action onAnimationEndCallback)
        {
            this.PlayerAnimator = giveActionInput.PlayerAnimator;

            PlayerAnimator = giveActionInput.PlayerAnimator;
            return PlayerAnimationPlayer.PlayItemGivenAnimation(giveActionInput.PlayerAnimator, this.AnimationConfiguration,
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
                var rightHandBoneTransform = BipedBoneRetriever.GetPlayerBone(BipedBone.RIGHT_HAND_CONTEXT, PlayerAnimator).transform;
                var scaleFactor = Vector3.one;
                ComponentSearchHelper.ComputeScaleFactorRecursively(rightHandBoneTransform, PlayerAnimator.transform, ref scaleFactor);
                DisplayedItemModel = GiveActionMiniatureInstanciate.Instance(this.AdventureGameConfigurationManager.ItemConf()[ItemGiven].ItemModel, rightHandBoneTransform, scaleFactor);
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