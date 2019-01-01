using UnityEngine;

public class PointOfInterestType : MonoBehaviour
{

    private AContextAction[] ContextActions;

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