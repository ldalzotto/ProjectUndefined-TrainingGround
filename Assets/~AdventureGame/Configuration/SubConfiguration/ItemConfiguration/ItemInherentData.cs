using UnityEngine;
using System.Collections;
using CoreGame;
using GameConfigurationID;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ItemInherentData", menuName = "Configuration/AdventureGame/ItemConfiguration/ItemInherentData", order = 1)]
    public class ItemInherentData : ScriptableObject
    {
        public Item ItemPrefab;
        public GameObject ItemModel;

        [CustomEnum(isCreateable: true)]
        public DiscussionTextID ItemReceivedDescriptionTextV2;
        public Sprite ItemMenuIcon;
    }

}
