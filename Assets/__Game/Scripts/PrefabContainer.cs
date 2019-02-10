using System.Collections.Generic;
using UnityEngine;

public class PrefabContainer : MonoBehaviour
{

    private static PrefabContainer instance;

    public GameObject ActionWheelNodePrefab;
    public GameObject InventoryMenuCellPrefab;
    public GameObject GiveActionMiniaturePrefab;

    [Header("Discussion UI Prefabs")]
    public DiscussionWindow DiscussionUIPrefab;
    public ChoicePopup ChoicePopupPrefab;
    public ChoicePopupText ChoicePopupTextPrefab;

    [Header("Player FX")]
    public TriggerableEffect PlayerSmokeEffectPrefab;

    [Header("Inventory Items Prefabs")]
    public Item IdCardItem;
    public Item CrowBarItem;

    [Header("Item Grab Popup")]
    public ItemReceivedPopup ItemReceivedPopup;

    [Header("Car Prefab")]
    public CarManager CarManagerPrefab;

    [Header("RT Puzzle Prefabs")]
    public LaunchProjectile ProjectilePrefab;

    public static Dictionary<ItemID, Item> InventoryItemsPrefabs;

    public static PrefabContainer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PrefabContainer>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        InventoryItemsPrefabs = new Dictionary<ItemID, Item>()
        {
            {ItemID.ID_CARD,  IdCardItem},
            {ItemID.CROWBAR,  CrowBarItem}
        };
    }

}
