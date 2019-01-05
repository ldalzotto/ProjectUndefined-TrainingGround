using UnityEngine;

public class GrabAction : AContextAction
{
    [Header("Item definition")]
    public Item Item;

    private GrabActionInput grabActionInput;
    private bool animationEnded;

    private InventoryEventManager InventoryEventManager;
    private PointOfInterestEventManager PointOfInterestEventManager;

    public override void OnStart()
    {
        InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
    }


    public override bool ComputeFinishedConditions()
    {
        if (animationEnded)
        {
            PointOfInterestEventManager.DestroyPOI(GetComponentInParent<PointOfInterestType>());
        }
        return animationEnded;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        animationEnded = false;
        grabActionInput = (GrabActionInput)ContextActionInput;
        StartCoroutine(AnimationPlayerHelper.Play(grabActionInput.PlayerAnimator, grabActionInput.PlayerAnimationEnum, 0f, () =>
        {
            InventoryEventManager.OnAddItem(grabActionInput.GrabbedItem);
            animationEnded = true;
        }));
    }

    public override void Tick(float d)
    {

    }


}

public class GrabActionInput : AContextActionInput
{

    private Animator playerAnimator;
    private PlayerAnimatioNnamesEnum playerAnimationEnum;
    private Item grabbedItem;

    public GrabActionInput(Animator playerAnimator, PlayerAnimatioNnamesEnum playerAnimationEnum, Item grabbedItem)
    {
        this.playerAnimator = playerAnimator;
        this.playerAnimationEnum = playerAnimationEnum;
        this.grabbedItem = grabbedItem;
    }

    public Animator PlayerAnimator { get => playerAnimator; }
    public Item GrabbedItem { get => grabbedItem; }
    public PlayerAnimatioNnamesEnum PlayerAnimationEnum { get => playerAnimationEnum; }
}