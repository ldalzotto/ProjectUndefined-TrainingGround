﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostsPOIManager : MonoBehaviour
{

    private Dictionary<PointOfInterestId, GhostPOI> ghostPOIs = null;

    public Dictionary<PointOfInterestId, GhostPOI> GhostPOIs
    {
        get
        {
            if (ghostPOIs == null)
            {
                Init();
            }
            return ghostPOIs;
        }
    }

    private void Init()
    {
        ghostPOIs = new Dictionary<PointOfInterestId, GhostPOI>();
        var poiIds = Enum.GetValues(typeof(PointOfInterestId)).Cast<PointOfInterestId>();
        foreach (var poiId in poiIds)
        {
            ghostPOIs.Add(poiId, new GhostPOI(poiId));
        }
    }

    public GhostPOI GetGhostPOI(PointOfInterestId pointOfInterestId)
    {
        return GhostPOIs[pointOfInterestId];
    }

    #region External Events
    public void OnScenePOICreated(PointOfInterestType pointOfInterestType)
    {
        pointOfInterestType.SyncFromGhostPOI(GhostPOIs[pointOfInterestType.PointOfInterestId]);
    }
    #endregion
}

public class GhostPOI
{
    public PointOfInterestId PointOfInterestId;
    private PointOfInterestScenarioState PointOfInterestScenarioState;
    private ContextActionSynchronizerManager contextActionSynchronizerManager;

    public PointOfInterestScenarioState PointOfInterestScenarioState1 { get => PointOfInterestScenarioState; }
    internal ContextActionSynchronizerManager ContextActionSynchronizerManager { get => contextActionSynchronizerManager; }

    public GhostPOI(PointOfInterestId PointOfInterestId)
    {
        PointOfInterestScenarioState = new PointOfInterestScenarioState();
        contextActionSynchronizerManager = new ContextActionSynchronizerManager();
    }

    public DiscussionTree GetAssociatedDiscussionTree()
    {
        return PointOfInterestScenarioState.DiscussionTree;
    }

    #region External Events
    public void OnGrabbableItemAdd(ItemID itemID, AContextAction contextActionToAdd)
    {
        contextActionSynchronizerManager.OnGrabbableItemAdded(itemID, contextActionToAdd);
    }
    public void OnGrabbableItemRemove(ItemID itemId)
    {
        contextActionSynchronizerManager.OnGrabbableItemRemoved(itemId);
    }
    public void OnReceivableItemAdd(ItemID itemID)
    {
        if (PointOfInterestScenarioState.ReceivableItemsComponent == null)
        {
            PointOfInterestScenarioState.ReceivableItemsComponent = new ReceivableItemsComponent();
        }
        PointOfInterestScenarioState.ReceivableItemsComponent.Add(itemID);
    }
    public void OnReceivableItemRemove(ItemID itemID)
    {
        if (PointOfInterestScenarioState.ReceivableItemsComponent != null)
        {
            PointOfInterestScenarioState.ReceivableItemsComponent.Remove(itemID);
        }
    }
    public void OnDiscussionTreeAdd(DiscussionTree discussionTree, AContextAction contextActionToAdd)
    {
        PointOfInterestScenarioState.DiscussionTree = discussionTree;
        contextActionSynchronizerManager.OnDiscussionTreeAdd(contextActionToAdd);
    }
    public void OnInteractableItemAdd(ItemID itemID)
    {
        if (PointOfInterestScenarioState.InteractableItemsComponent == null)
        {
            PointOfInterestScenarioState.InteractableItemsComponent = new InteractableItemsComponent();
        }
        PointOfInterestScenarioState.InteractableItemsComponent.Add(itemID);
    }
    public void OnInteractableItemRemove(ItemID itemID)
    {
        if (PointOfInterestScenarioState.InteractableItemsComponent != null)
        {
            PointOfInterestScenarioState.InteractableItemsComponent.Remove(itemID);
        }
    }
    public void OnLevelZoneTransitionAdd(LevelZonesID levelZonesID, AContextAction contextActionToAdd)
    {
        contextActionSynchronizerManager.OnLevelTransitionAdd(contextActionToAdd);
    }
    #endregion
}


#region Context Action Synchronizer
[System.Serializable]
class ContextActionSynchronizerManager
{
    private Dictionary<string, AContextAction> contextActions = new Dictionary<string, AContextAction>();

    public List<AContextAction> ContextActions
    {
        get
        {
            return contextActions.Values.ToList();
        }
    }

    public void OnGrabbableItemAdded(ItemID itemId, AContextAction contextActionToAdd)
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

    public void OnLevelTransitionAdd(AContextAction contextActionToAdd)
    {
        var key = typeof(LevelZoneTransitionAction).ToString();
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

    public Dictionary<string, AContextAction> GetRawContextActions()
    {
        return contextActions;
    }

    public void ReplaceContextActions(Dictionary<string, AContextAction> contextActions)
    {
        this.contextActions = contextActions;
    }

}
#endregion
