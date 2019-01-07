using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject ItemModel;
    public ItemID ItemID;

    private AContextAction[] contextActions;

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

[System.Serializable]
public enum ItemID
{
    DUMMY_ITEM = 0,
    ID_CARD = 1
}