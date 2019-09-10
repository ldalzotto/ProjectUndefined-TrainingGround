using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class PrefabContainer : MonoBehaviour
    {

        private static PrefabContainer instance;

        public GameObject ActionWheelNodePrefab;
        public GameObject InventoryMenuCellPrefab;
        public GameObject GiveActionMiniaturePrefab;

        [Header("Player FX")]
        public TriggerableEffect PlayerSmokeEffectPrefab;

        [Header("Item Grab Popup")]
        public ItemReceivedPopup ItemReceivedPopup;
        
        public static PrefabContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AdventureGameSingletonInstances.AdventurePrefabContainer;
                }
                return instance;
            }
        }

    }

}