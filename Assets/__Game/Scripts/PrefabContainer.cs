using System.Collections.Generic;
using UnityEngine;

public class PrefabContainer : MonoBehaviour
{

    private static PrefabContainer instance;

    public GameObject ActionWheelNodePrefab;
    public GameObject InventoryMenuCellPrefab;
    public GameObject GiveActionMiniaturePrefab;
    public Discussion DiscussionUIPrefab;

    [Header("Player FX")]
    public TriggerableEffect PlayerSmokeEffectPrefab;

    [Header("Inventory Items Prefabs")]
    public Item IdCardItem;
    public Item IdCardV2Item;

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
            {ItemID.ID_CARD_V2, IdCardV2Item}
        };
    }

}
