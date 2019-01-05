using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestScenarioState : MonoBehaviour
{
    public ReceivableItemsComponent ReceivableItemsComponent;
}

[System.Serializable]
public class ReceivableItemsComponent
{
    public List<Item> receivableItems;

    public bool IsElligibleToGiveItem(Item itemToGive)
    {
        foreach (var receivableItem in receivableItems)
        {
            if (receivableItem.name == itemToGive.name)
            {
                return true;
            }
        }
        return false;
    }

}
