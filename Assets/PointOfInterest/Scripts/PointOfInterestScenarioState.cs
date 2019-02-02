using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestScenarioState : MonoBehaviour
{
    public ReceivableItemsComponent ReceivableItemsComponent;
    public DiscussionTree DiscussionTree;
    public InteractableItemsComponent InteractableItemsComponent;
}

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

#region Interactable Items
[System.Serializable]
public class InteractableItemsComponent
{
    public List<ItemID> interactableItems = new List<ItemID>();

    public bool IsElligibleToInteractWithItem(Item itemToGive)
    {
        foreach (var interactableItem in interactableItems)
        {
            if (interactableItem == itemToGive.ItemID)
            {
                return true;
            }
        }
        return false;
    }

    public void AddItemID(ItemID itemID)
    {
        interactableItems.Add(itemID);
    }

    public void RemoveItemID(ItemID itemID)
    {
        interactableItems.Remove(itemID);
    }
}
#endregion
