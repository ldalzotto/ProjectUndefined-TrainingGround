using UnityEngine;

public class GrabAction : AContextAction
{
    public Item Item;

    private GrabActionInput grabActionInput;
    private bool animationEnded;

    private InventoryEventManager InventoryEventManager;
    private PointOfInterestEventManager PointOfInterestEventManager;

    #region Internal Dependencies
    private PointOfInterestType associatedPOI;

    public PointOfInterestType AssociatedPOI { get => associatedPOI; }

    #endregion

    public GrabAction(ItemID itemId, AContextAction nextAction) : base(nextAction)
    {
        Item = PrefabContainer.InventoryItemsPrefabs[itemId];
        InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
    }

    public override bool ComputeFinishedConditions()
    {
        return animationEnded;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        animationEnded = false;
        grabActionInput = (GrabActionInput)ContextActionInput;
        this.associatedPOI = grabActionInput.TargetedPOI;
        this.associatedPOI.StartCoroutine(AnimationPlayerHelper.Play(grabActionInput.PlayerAnimator, grabActionInput.PlayerAnimationEnum, 0f, () =>
         {
             InventoryEventManager.OnAddItem(grabActionInput.GrabbedItem);
             animationEnded = true;
         }));
    }

    public override void Tick(float d)
    {

    }

    public override void AfterFinishedEventProcessed()
    {
        PointOfInterestEventManager.DestroyPOI(this.associatedPOI);
    }
}

public class GrabActionInput : AContextActionInput
{
    private PointOfInterestType targetedPOI;
    private Animator playerAnimator;
    private PlayerAnimatioNnamesEnum playerAnimationEnum;
    private Item grabbedItem;

    public GrabActionInput(PointOfInterestType targetedPOI, Animator playerAnimator, PlayerAnimatioNnamesEnum playerAnimationEnum, Item grabbedItem)
    {
        this.targetedPOI = targetedPOI;
        this.playerAnimator = playerAnimator;
        this.playerAnimationEnum = playerAnimationEnum;
        this.grabbedItem = grabbedItem;
    }

    public Animator PlayerAnimator { get => playerAnimator; }
    public Item GrabbedItem { get => grabbedItem; }
    public PlayerAnimatioNnamesEnum PlayerAnimationEnum { get => playerAnimationEnum; }
    public PointOfInterestType TargetedPOI { get => targetedPOI; }
}