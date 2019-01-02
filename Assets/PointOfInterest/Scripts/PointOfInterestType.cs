using UnityEngine;

public class PointOfInterestType : MonoBehaviour
{
    [SerializeField]
    private float maxDistanceToInteractWithPlayer;

    private AContextAction[] ContextActions;

    public float MaxDistanceToInteractWithPlayer { get => maxDistanceToInteractWithPlayer; }

    private void Start()
    {
        var childActions = GetComponentsInChildren(typeof(AContextAction));
        ContextActions = new AContextAction[childActions.Length];

        for (var i = 0; i < ContextActions.Length; i++)
        {
            ContextActions[i] = (AContextAction)childActions[i];
        }

    }

}