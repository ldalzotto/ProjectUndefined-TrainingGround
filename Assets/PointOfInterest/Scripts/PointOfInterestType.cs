using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointOfInterestType : MonoBehaviour
{
    public PointOfInterestId PointOfInterestId;
    public float MaxDistanceToInteractWithPlayer;

    #region Internal Depencies
    private PointOfInterestScenarioState pointOfInterestScenarioState;
    #endregion

    private ContextActionSynchronizerManager ContextActionSynchronizerManager;

    private void Start()
    {
        #region External Dependencies
        var PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
        #endregion
        this.ContextActionSynchronizerManager = new ContextActionSynchronizerManager();
        this.pointOfInterestScenarioState = gameObject.AddComponent<PointOfInterestScenarioState>();
        PointOfInterestEventManager.OnPOICreated(this);
    }

    #region Logical Conditions
    public bool IsElligibleToGiveItem(Item itemToGive)
    {
        return pointOfInterestScenarioState != null && pointOfInterestScenarioState.ReceivableItemsComponent.IsElligibleToGiveItem(itemToGive);
    }
    #endregion

    #region External Events
    public void OnGrabbableItemAdd(ItemID itemId)
    {
        if (pointOfInterestScenarioState.GabbableItemsComponent == null)
        {
            pointOfInterestScenarioState.GabbableItemsComponent = new GabbableItemsComponent();
        }
        pointOfInterestScenarioState.GabbableItemsComponent.AddItemID(itemId);
        ContextActionSynchronizerManager.OnGrabbableItemAdded(this, itemId);
    }
    public void OnGrabbableItemRemove(ItemID itemId)
    {
        pointOfInterestScenarioState.GabbableItemsComponent.RemoveItemID(itemId);
        if (ContextActionSynchronizerManager.OnGrabbableItemRemoved(itemId))
        {
            pointOfInterestScenarioState.GabbableItemsComponent = null;
        }
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

}

public enum PointOfInterestId
{
    NONE = 0,
    BOUNCER = 1,
    ID_CARD = 2,
    ID_CARD_V2 = 3
}

#region Context Action Synchronizer

[System.Serializable]
class ContextActionSynchronizerManager
{
    private GameObject contextActionContainer;

    private Dictionary<string, List<AContextAction>> contextActions = new Dictionary<string, List<AContextAction>>();


    public List<AContextAction> ContextActions
    {
        get
        {
            return contextActions.Values.SelectMany(d => d).ToList();
        }
    }

    public void OnGrabbableItemAdded(PointOfInterestType pointOfInterestType, ItemID itemID)
    {
        if (!contextActions.ContainsKey(typeof(GrabAction).ToString()))
        {
            contextActions.Add(typeof(GrabAction).ToString(), new List<AContextAction>() { new GrabAction(itemID, pointOfInterestType) });
        }
        else
        {
            contextActions[typeof(GrabAction).ToString()].Add(new GrabAction(itemID, pointOfInterestType));
        }

    }

    public bool OnGrabbableItemRemoved(ItemID itemID)
    {
        GrabAction contextActionToRemove = null;
        var grabContextActions = contextActions[typeof(GrabAction).ToString()];
        foreach (GrabAction grabContextAction in grabContextActions)
        {
            if (grabContextAction.Item.ItemID == itemID)
            {
                contextActionToRemove = grabContextAction;
            }
        }
        if (contextActionToRemove != null)
        {
            grabContextActions.Remove(contextActionToRemove);
        }
        return (grabContextActions.Count == 0);
    }

}
#endregion
