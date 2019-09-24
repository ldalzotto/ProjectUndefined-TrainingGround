using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class ItemInteractAction : AContextAction
    {

        [SerializeField]
        private ItemID involvedItem;

        [NonSerialized]
        private bool InteractionResolved;

        public ItemInteractAction(ItemID involvedItem, List<SequencedAction> nextActionInteractionAllowed, SelectionWheelNodeConfigurationId SelectionWheelNodeConfigurationId) : base(nextActionInteractionAllowed, SelectionWheelNodeConfigurationId)
        {
            this.involvedItem = involvedItem;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return InteractionResolved;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            #region External Dependencies
            var animationConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.AnimationConfiguration();
            #endregion

            InteractionResolved = false;

            var interactActionInput = (InteractActionInput)ContextActionInput;
            if (IsElligibleToInteract(interactActionInput))
            {
                OnInteractionResolved();
            }
            else
            {
                interactActionInput.PlayerManager.StartCoroutine(AnimationPlayerHelper.PlayAndWait(interactActionInput.PlayerAnimator, animationConfiguration.ConfigurationInherentData[AnimationID.ACTION_FORBIDDEN], 0f, () =>
               {
                   this.OnInteractionResolved();
                   return null;
               }));
                //abort context action chain
                SetNextContextAction(null);
            }
        }

        private bool IsElligibleToInteract(InteractActionInput interactActionInput)
        {
            return (interactActionInput != null && interactActionInput.TargetedPOI != null && interactActionInput.TargetedPOI.IsInteractableWithItem(involvedItem));
        }

        private void OnInteractionResolved()
        {
            InteractionResolved = true;
        }

        public override void Tick(float d)
        {
        }
    }

    public class InteractActionInput : AContextActionInput
    {
        private PointOfInterestType targetedPOI;
        private Animator playerAnimator;
        private PlayerManager playerManager;

        public InteractActionInput(PointOfInterestType targetedPOI, PlayerManager playerManager)
        {
            this.targetedPOI = targetedPOI;
            this.playerManager = playerManager;
            this.playerAnimator = playerManager.GetPlayerAnimator();
        }

        public PointOfInterestType TargetedPOI { get => targetedPOI; }
        public Animator PlayerAnimator { get => playerAnimator; }
        public PlayerManager PlayerManager { get => playerManager; }
    }
}