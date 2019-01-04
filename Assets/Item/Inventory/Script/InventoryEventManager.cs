using UnityEngine;

public class InventoryEventManager : MonoBehaviour
{

    private InventoryManager InventoryManager;
    private Inventory Inventory;
    private PlayerManager PlayerManager;

    private void Start()
    {
        InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        Inventory = GameObject.FindObjectOfType<Inventory>();
    }

    public void OnAddItem(Item item)
    {
        InventoryManager.OnAddItem(item);
    }

    public void OnInventoryEnabled()
    {
        Debug.Log("Inventory enabled");
        PlayerManager.OnInventoryEnabled();
        StartCoroutine(InventoryManager.OnInventoryEnabled());
        Inventory.OnInventoryEnabled();
    }

    public void OnInventoryDisabled()
    {
        Debug.Log("Inventory disabled");
        PlayerManager.OnInventoryDisabled();
        InventoryManager.OnInventoryDisabled();
        Inventory.OnInventoryDisabled();
    }

}
