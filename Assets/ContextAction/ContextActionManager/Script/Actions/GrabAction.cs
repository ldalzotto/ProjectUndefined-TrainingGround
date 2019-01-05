﻿using System.Collections;
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
        InventoryEventManager.OnAddItem(grabActionInput.GrabbedItem);
        animationEnded = true;
    }

}

public class GrabActionInput : AContextActionInput
{

    private Animator playerAnimator;
    private string animationName;
    private int animationLayerIndex;
    private Item grabbedItem;

    public GrabActionInput(Animator playerAnimator, string animationName, int animationLayerIndex, Item grabbedItem)
    {
        this.playerAnimator = playerAnimator;
        this.animationName = animationName;
        this.animationLayerIndex = animationLayerIndex;
        this.grabbedItem = grabbedItem;
    }

    public Animator PlayerAnimator { get => playerAnimator; }
    public string AnimationName { get => animationName; }
    public int LayerIndex { get => animationLayerIndex; }
    public Item GrabbedItem { get => grabbedItem; }
}