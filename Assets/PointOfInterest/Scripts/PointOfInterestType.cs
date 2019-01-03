using UnityEngine;

public class PointOfInterestType : MonoBehaviour
{
    [SerializeField]
    private float maxDistanceToInteractWithPlayer;

    [Header("Item definition")]
    public Item Item;

    private AContextAction[] contextActions;

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

    }

}