using UnityEngine;

public class InventoryEventManager : MonoBehaviour
{

    private InventoryManager InventoryManager;
    private PlayerManager PlayerManager;

    private void Start()
    {
        InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
    }

    public void OnInventoryEnabled()
    {
        Debug.Log("Inventory enabled");
        PlayerManager.OnInventoryEnabled();
        StartCoroutine(InventoryManager.OnInventoryEnabled());
    }

    public void OnInventoryDisabled()
    {
        Debug.Log("Inventory disabled");
        PlayerManager.OnInventoryDisabled();
        InventoryManager.OnInventoryDisabled();
    }

}
