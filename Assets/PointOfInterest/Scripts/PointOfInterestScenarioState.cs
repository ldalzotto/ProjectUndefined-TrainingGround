using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestScenarioState : MonoBehaviour
{
    public GabbableItemsComponent GabbableItemsComponent;
    public ReceivableItemsComponent ReceivableItemsComponent;
    public DiscussionTree DiscussionTree;
}

#region Grabbable Items
[System.Serializable]
public class GabbableItemsComponent
{
    public List<ItemID> grabbableItems = new List<ItemID>();

    public void AddItemID(ItemID itemID)
    {
        grabbableItems.Add(itemID);
    }

    public void RemoveItemID(ItemID itemID)
    {
        grabbableItems.Remove(itemID);
    }

}

#endregion

#region Receive Items
[System.Serializable]
public class ReceivableItemsComponent
{
    public List<ItemID> receivableItems = new List<ItemID>();

    public bool IsElligibleToGiveItem(Item itemToGive)
    {
        foreach (var receivableItem in receivableItems)
        {
            if (receivableItem == itemToGive.ItemID)
            {
                return true;
            }
        }
        return false;
    }

    public void AddItemID(ItemID itemID)
    {
        receivableItems.Add(itemID);
    }

    public void RemoveItemID(ItemID itemID)
    {
        receivableItems.Remove(itemID);
    }

}
#endregion
