using CoreGame;
using UnityEngine;

namespace AdventureGame
{

    public class InteractAction : AContextAction
    {

        private ItemID involvedItem;
        private bool InteractionResolved;

        public InteractAction(ItemID involvedItem, AContextAction nextActionInteractionAllowed) : base(nextActionInteractionAllowed)
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

        public override void FirstExecutionAction(AContextActionInput ContextActionInput)
        {
            InteractionResolved = false;

            var interactActionInput = (InteractActionInput)ContextActionInput;
            if (IsElligibleToInteract(interactActionInput))
            {
                OnInteractionResolved();
            }
            else
            {
                interactActionInput.PlayerManager.StartCoroutine(AnimationPlayerHelper.Play(interactActionInput.PlayerAnimator, PlayerAnimatioNamesEnum.PLAYER_ACTION_FORBIDDEN, 0f, () =>
                {
                    this.OnInteractionResolved();
                    return null;
                }));
                //abort context action chain
                NextContextAction = null;
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