using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestScenarioState : MonoBehaviour
{
    public ReceivableItemsComponent ReceivableItemsComponent;
}

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
