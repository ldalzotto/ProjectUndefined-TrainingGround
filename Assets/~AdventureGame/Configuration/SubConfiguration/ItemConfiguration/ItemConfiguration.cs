using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ItemConfiguration", menuName = "Configuration/AdventureGame/ItemConfiguration/ItemConfiguration", order = 1)]
    public class ItemConfiguration : ConfigurationSerialization<ItemID, ItemInherentData>
    {
    }
}
