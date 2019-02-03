using UnityEngine;

public class InventoryEventManager : MonoBehaviour
{

    private InventoryManager InventoryManager;
    private InventoryMenu InventoryMenu;
    private PlayerManager PlayerManager;

    public void Init()
    {
        InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
    }

    public void OnAddItem(Item item)
    {
        InventoryManager.OnAddItem(item);
    }

    public void OnInventoryEnabled()
    {
        PlayerManager.OnInventoryEnabled();
        StartCoroutine(InventoryManager.OnInventoryEnabled());
        InventoryMenu.OnInventoryEnabled();
    }

    public void OnInventoryDisabled()
    {
        PlayerManager.OnInventoryDisabled();
        InventoryManager.OnInventoryDisabled();
        InventoryMenu.OnInventoryDisabled();
    }

    public void OnItemGiven(Item givenItem)
    {
        InventoryManager.OnItemGiven(givenItem);
        InventoryMenu.OnItemDeleted(givenItem);
    }

}
