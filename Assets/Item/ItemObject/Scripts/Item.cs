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
    NONE = 0,
    DUMMY_ITEM = 1,
    ID_CARD = 2,
    ID_CARD_V2 = 3
}