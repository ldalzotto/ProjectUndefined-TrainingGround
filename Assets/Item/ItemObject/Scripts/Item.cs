using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string ItemID;
    private ItemID itemID;

    public void OnValidate()
    {
        itemID = (ItemID)Enum.Parse(typeof(ItemID), ItemID);
    }

}

[System.Serializable]
public enum ItemID
{
    DUMMY_ITEM
}