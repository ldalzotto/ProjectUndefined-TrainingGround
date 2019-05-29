using UnityEngine;
using System.Collections;
using ConfigurationEditor;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ItemConfiguration", menuName = "Configuration/AdventureGame/ItemConfiguration/ItemConfiguration", order = 1)]
    public class ItemConfiguration : ConfigurationSerialization<ItemID, ItemInherentData>
    {
    }

    [System.Serializable]
    public enum ItemID
    {
        NONE = 0,
        DUMMY_ITEM = 1,
        ID_CARD = 2,
        CROWBAR = 5
    }


}
