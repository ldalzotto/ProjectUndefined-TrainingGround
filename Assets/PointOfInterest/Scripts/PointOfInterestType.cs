using UnityEngine;

public class PointOfInterestType : MonoBehaviour
{
    public PointOfInterestId PointOfInterestId;
    public float MaxDistanceToInteractWithPlayer;
    private AContextAction[] contextActions;

    #region Internal Depencies
    private PointOfInterestScenarioState pointOfInterestScenarioState;
    #endregion



    public AContextAction[] ContextActions { get => contextActions; }
    public PointOfInterestScenarioState PointOfInterestScenarioState { get => pointOfInterestScenarioState; }

    private void Start()
    {
        #region External Dependencies
        var PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
        #endregion
        this.pointOfInterestScenarioState = gameObject.AddComponent<PointOfInterestScenarioState>();
        PointOfInterestEventManager.OnPOICreated(this);

        var childActions = GetComponentsInChildren(typeof(AContextAction));
        contextActions = new AContextAction[childActions.Length];

        for (var i = 0; i < contextActions.Length; i++)
        {
            contextActions[i] = (AContextAction)childActions[i];
        }
    }

    #region Logical Conditions
    public bool IsElligibleToGiveItem(Item itemToGive)
    {
        return pointOfInterestScenarioState != null && pointOfInterestScenarioState.ReceivableItemsComponent.IsElligibleToGiveItem(itemToGive);
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