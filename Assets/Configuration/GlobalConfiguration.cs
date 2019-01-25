
using System.Collections.Generic;

#region Point Of Interest Configuraiton
public enum PointOfInterestId
{
    NONE = 0,
    BOUNCER = 1,
    ID_CARD = 2,
    ID_CARD_V2 = 3,
    PLAYER = 4,
    DUMBSTER = 5
}
#endregion

#region Item Configuration
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
#endregion