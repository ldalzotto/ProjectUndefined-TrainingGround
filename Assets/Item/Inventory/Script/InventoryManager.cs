using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private HashSet<Item> holdItems = new HashSet<Item>();

    public void AddItem(Item item)
    {
        holdItems.Add(item);
    }

}