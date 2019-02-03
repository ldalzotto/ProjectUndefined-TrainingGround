using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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

    public PointOfInterestScenarioState PointOfInterestScenarioState { get => pointOfInterestScenarioState; }

    private void Start()
    {
        #region External Dependencies
        var PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
        #endregion
        this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
        this.pointOfInterestScenarioState = new PointOfInterestScenarioState();
        this.PointOfInterestContextData = GetComponentInChildren<PointOfInterestContextDataContainer>();
        PointOfInterestEventManager.OnPOICreated(this);
    }

    private void OnDrawGizmos()
    {
        var labelStyle = GUI.skin.GetStyle("Label");
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.red;
#if UNITY_EDITOR
        Handles.Label(transform.position, PointOfInterestId.ToString(), labelStyle);
#endif

        Gizmos.DrawIcon(transform.position + new Vector3(0, 1.5f, 0), "Gizmo_POI", true);
    }

    #region Logical Conditions
    public bool IsElligibleToGiveItem(Item itemToGive)
    {
        return pointOfInterestScenarioState != null && pointOfInterestScenarioState.ReceivableItemsComponent != null && pointOfInterestScenarioState.ReceivableItemsComponent.IsElligible(itemToGive.ItemID);
    }
    public bool IsInteractableWithItem(Item involvedItem)
    {
        return pointOfInterestScenarioState != null && pointOfInterestScenarioState.InteractableItemsComponent != null && pointOfInterestScenarioState.InteractableItemsComponent.IsElligible(involvedItem.ItemID);
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
        pointOfInterestScenarioState.ReceivableItemsComponent.Add(itemID);
    }
    public void OnReceivableItemRemove(ItemID itemID)
    {
        if (pointOfInterestScenarioState.ReceivableItemsComponent != null)
        {
            pointOfInterestScenarioState.ReceivableItemsComponent.Remove(itemID);
        }
    }
    public void OnDiscussionTreeAdd(DiscussionTree discussionTree, AContextAction contextActionToAdd)
    {
        pointOfInterestScenarioState.DiscussionTree = discussionTree;
        ContextActionSynchronizerManager.OnDiscussionTreeAdd(contextActionToAdd);
    }
    public void OnInteractableItemAdd(ItemID itemID)
    {
        if (pointOfInterestScenarioState.InteractableItemsComponent == null)
        {
            pointOfInterestScenarioState.InteractableItemsComponent = new InteractableItemsComponent();
        }
        pointOfInterestScenarioState.InteractableItemsComponent.Add(itemID);
    }
    public void OnInteractableItemRemove(ItemID itemID)
    {
        if (pointOfInterestScenarioState.InteractableItemsComponent != null)
        {
            pointOfInterestScenarioState.InteractableItemsComponent.Remove(itemID);
        }
    }
    public void OnLevelZoneTransitionAdd(LevelZonesID levelZonesID, AContextAction contextActionToAdd)
    {
        ContextActionSynchronizerManager.OnLevelTransitionAdd(contextActionToAdd);
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

    public void ReplacePointOfInterestState(PointOfInterestScenarioState newState)
    {
        this.pointOfInterestScenarioState = newState;
    }

    public Dictionary<string, AContextAction> GetRawContextActions()
    {
        return ContextActionSynchronizerManager.GetRawContextActions();
    }

    public void ReplaceContextActions(Dictionary<string, AContextAction> contextActions)
    {
        ContextActionSynchronizerManager.ReplaceContextActions(contextActions);
    }
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
