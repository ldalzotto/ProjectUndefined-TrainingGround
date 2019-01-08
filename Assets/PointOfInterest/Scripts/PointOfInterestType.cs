using UnityEngine;

public class PointOfInterestType : MonoBehaviour
{
    public PointOfInterestId PointOfInterestId;
    public float MaxDistanceToInteractWithPlayer;
    private AContextAction[] contextActions;

    #region Internal Depencies
    private PointOfInterestScenarioState PointOfInterestScenarioState;
    #endregion



    public AContextAction[] ContextActions { get => contextActions; }

    private void Start()
    {
        #region External Dependencies
        var PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
        #endregion
        PointOfInterestEventManager.OnPOICreated(this);

        var childActions = GetComponentsInChildren(typeof(AContextAction));
        contextActions = new AContextAction[childActions.Length];

        for (var i = 0; i < contextActions.Length; i++)
        {
            contextActions[i] = (AContextAction)childActions[i];
        }

        PointOfInterestScenarioState = GetComponent<PointOfInterestScenarioState>();
    }

    #region Logical Conditions
    public bool IsElligibleToGiveItem(Item itemToGive)
    {
        return PointOfInterestScenarioState != null && PointOfInterestScenarioState.ReceivableItemsComponent.IsElligibleToGiveItem(itemToGive);
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

}

public enum PointOfInterestId
{
    NONE = 0,
    BOUNCER = 1,
    ID_CARD = 2
}