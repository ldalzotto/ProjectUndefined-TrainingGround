using UnityEngine;

public class ItemResourceResolver
{

    private const string ITEM_ICON_RESOURCE_BASE_PATH = "Item/ItemObject/Icon/";

    public static Sprite ResolveItemInventoryIcon(Item item)
    {
        return Resources.Load<Sprite>(ITEM_ICON_RESOURCE_BASE_PATH + item.name + "_icon");
    }
}
