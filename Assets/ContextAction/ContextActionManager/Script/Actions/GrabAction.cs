using System.Collections;
using UnityEngine;

public class GrabAction : AContextAction
{
    private GrabActionInput grabActionInput;
    private bool animationEnded;

    private InventoryManager InventoryManager;

    public override void OnStart()
    {
        InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
    }


    public override bool ComputeFinishedConditions()
    {
        if (animationEnded)
        {
            StartCoroutine(DostroyPOICoroutine());
        }
        return animationEnded;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        animationEnded = false;
        grabActionInput = (GrabActionInput)ContextActionInput;
        StartCoroutine(GrabActionCoroutine());
    }

    public override void Tick(float d)
    {

    }

    private IEnumerator GrabActionCoroutine()
    {
        grabActionInput.PlayerAnimator.Play(grabActionInput.AnimationName);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(grabActionInput.PlayerAnimator, grabActionInput.AnimationName, grabActionInput.LayerIndex);
        InventoryManager.AddItem(grabActionInput.GrabbedItem);
        animationEnded = true;
    }

    private IEnumerator DostroyPOICoroutine()
    {
        yield return new WaitForEndOfFrame();
        Destroy(transform.parent.gameObject);
    }

}

public class GrabActionInput : AContextActionInput
{

    private Animator playerAnimator;
    private string animationName;
    private int layerIndex;
    private Item grabbedItem;

    public GrabActionInput(Animator playerAnimator, string animationName, int layerIndex, Item grabbedItem)
    {
        this.playerAnimator = playerAnimator;
        this.animationName = animationName;
        this.layerIndex = layerIndex;
        this.grabbedItem = grabbedItem;
    }

    public Animator PlayerAnimator { get => playerAnimator; }
    public string AnimationName { get => animationName; }
    public int LayerIndex { get => layerIndex; }
    public Item GrabbedItem { get => grabbedItem; }
}