using System.Collections.Generic;
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
    public void SyncFromGhostPOI(GhostPOI ghostPOI)
    {
        pointOfInterestScenarioState = ghostPOI.PointOfInterestScenarioState1;
        ContextActionSynchronizerManager = ghostPOI.ContextActionSynchronizerManager;
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

    public DiscussionTree GetAssociatedDiscussionTree()
    {
        return pointOfInterestScenarioState.DiscussionTree;
    }

    public PointOfInterestContextDataContainer GetContextData()
    {
        return PointOfInterestContextData;
    }

    internal List<AContextAction> GetContextActions()
    {
        return ContextActionSynchronizerManager.ContextActions;
    }
}
