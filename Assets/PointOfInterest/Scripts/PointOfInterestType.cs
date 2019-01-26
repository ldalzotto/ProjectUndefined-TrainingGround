﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointOfInterestType : MonoBehaviour
{
    public PointOfInterestId PointOfInterestId;
    public bool InteractionWithPlayerAllowed = true;
    public float MaxDistanceToInteractWithPlayer;

    #region Internal Depencies
    private PointOfInterestScenarioState pointOfInterestScenarioState;
    private PointOfInterestContextDataContainer PointOfInterestContextData;
    #endregion

    private ContextActionSynchronizerManager ContextActionSynchronizerManager;

    private void Start()
    {
        #region External Dependencies
        var PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
        #endregion
        this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
        this.pointOfInterestScenarioState = gameObject.AddComponent<PointOfInterestScenarioState>();
        this.PointOfInterestContextData = GetComponentInChildren<PointOfInterestContextDataContainer>();
        PointOfInterestEventManager.OnPOICreated(this);
    }

    #region Logical Conditions
    public bool IsElligibleToGiveItem(Item itemToGive)
    {
        return pointOfInterestScenarioState != null && pointOfInterestScenarioState.ReceivableItemsComponent.IsElligibleToGiveItem(itemToGive);
    }
    #endregion

    #region External Events
    public void OnGrabbableItemAdd(ItemID itemID, AContextAction contextActionToAdd)
    {
        ContextActionSynchronizerManager.OnGrabbableItemAdded(this, itemID, contextActionToAdd);
    }
    public void OnGrabbableItemRemove(ItemID itemId)
    {
        ContextActionSynchronizerManager.OnGrabbableItemRemoved(itemId);
    }
    public void OnReceivableItemAdd(ItemID itemID)
    {
        if (pointOfInterestScenarioState.ReceivableItemsComponent == null)
        {
            pointOfInterestScenarioState.ReceivableItemsComponent = new ReceivableItemsComponent();
        }
        pointOfInterestScenarioState.ReceivableItemsComponent.AddItemID(itemID);
    }
    public void OnReceivableItemRemove(ItemID itemID)
    {
        if (pointOfInterestScenarioState.ReceivableItemsComponent != null)
        {
            pointOfInterestScenarioState.ReceivableItemsComponent.RemoveItemID(itemID);
        }
    }
    public void OnDiscussionTreeAdd(DiscussionTree discussionTree, AContextAction contextActionToAdd)
    {
        pointOfInterestScenarioState.DiscussionTree = discussionTree;
        ContextActionSynchronizerManager.OnDiscussionTreeAdd(contextActionToAdd);
    }
    #endregion

    #region Prefab Data Retrieval
    public Renderer[] GetRenderers()
    {
        var parentObject = transform.parent;
        return parentObject.GetComponentsInChildren<Renderer>();
    }
    public GameObject GetRootObject()
    {
        return transform.parent.gameObject;
    }
    #endregion

    public List<AContextAction> GetContextActions()
    {
        return ContextActionSynchronizerManager.ContextActions;
    }

    public DiscussionTree GetAssociatedDiscussionTree()
    {
        return pointOfInterestScenarioState.DiscussionTree;
    }

    public PointOfInterestContextDataContainer GetContextData()
    {
        return PointOfInterestContextData;
    }

}

#region Context Action Synchronizer

[System.Serializable]
class ContextActionSynchronizerManager
{
    private GameObject contextActionContainer;

    private Dictionary<string, AContextAction> contextActions = new Dictionary<string, AContextAction>();


    public List<AContextAction> ContextActions
    {
        get
        {
            return contextActions.Values.ToList();
        }
    }

    public void OnGrabbableItemAdded(PointOfInterestType pointOfInterestType, ItemID itemId, AContextAction contextActionToAdd)
    {
        var key = itemId.ToString();
        ContextActionAddSilently(contextActionToAdd, key);
    }

    public void OnGrabbableItemRemoved(ItemID itemID)
    {
        contextActions.Remove(itemID.ToString());
    }

    public void OnDiscussionTreeAdd(AContextAction contextActionToAdd)
    {
        var key = typeof(TalkAction).ToString();
        ContextActionAddSilently(contextActionToAdd, key);
    }

    private void ContextActionAddSilently(AContextAction contextActionToAdd, string key)
    {
        if (contextActions.ContainsKey(key))
        {
            contextActions[key] = contextActionToAdd;
        }
        else
        {
            contextActions.Add(key, contextActionToAdd);
        }
    }

}
#endregion
