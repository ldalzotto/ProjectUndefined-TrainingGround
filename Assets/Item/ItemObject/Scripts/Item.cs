using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject ItemModel;
    public ItemID ItemID;

    private List<AContextAction> contextActions;

    public List<AContextAction> ContextActions { get => contextActions; }

    private void Start()
    {
        contextActions = ItemContextActionBuilder.BuilItemContextActions(this);
    }
}

[System.Serializable]
public enum ItemID
{
    NONE = 0,
    DUMMY_ITEM = 1,
    ID_CARD = 2,
    ID_CARD_V2 = 3,
    ID_CARD_V3 = 4
}



public class ItemContextActionBuilder
{
    public static List<AContextAction> BuilItemContextActions(Item item)
    {
        switch (item.ItemID)
        {
            case ItemID.ID_CARD:
                return new List<AContextAction>() { new GiveAction(item, new DummyContextAction(null)) };
            case ItemID.ID_CARD_V2:
                return new List<AContextAction>() { new GiveAction(item, new DummyContextAction(null)) };
        }
        return new List<AContextAction>();
    }
}