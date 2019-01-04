using UnityEngine;

public class InventoryEventManager : MonoBehaviour
{

    private InventoryManager InventoryManager;
    private InventoryMenu InventoryMenu;
    private PlayerManager PlayerManager;

    private void Start()
    {
        InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
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

}
