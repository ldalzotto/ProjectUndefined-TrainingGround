using UnityEngine;

public class PointOfInterestType : MonoBehaviour
{
    [SerializeField]
    private float maxDistanceToInteractWithPlayer;

    private AContextAction[] contextActions;
    private PointOfInterestScenarioState PointOfInterestScenarioState;

    public float MaxDistanceToInteractWithPlayer { get => maxDistanceToInteractWithPlayer; }
    public AContextAction[] ContextActions { get => contextActions; }

    private void Start()
    {
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