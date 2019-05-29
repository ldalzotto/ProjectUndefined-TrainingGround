using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ItemInherentData", menuName = "Configuration/AdventureGame/ItemConfiguration/ItemInherentData", order = 1)]
    public class ItemInherentData : ScriptableObject
    {
        public Item ItemPrefab;
        public GameObject ItemModel;
        public string ItemReceivedDescriptionText;
    }

}
