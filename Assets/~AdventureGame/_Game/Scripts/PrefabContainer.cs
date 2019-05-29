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

        [Header("Discussion UI Prefabs")]
        public DiscussionWindow DiscussionUIPrefab;
        public ChoicePopup ChoicePopupPrefab;
        public ChoicePopupText ChoicePopupTextPrefab;

        [Header("Player FX")]
        public TriggerableEffect PlayerSmokeEffectPrefab;

        [Header("Item Grab Popup")]
        public ItemReceivedPopup ItemReceivedPopup;

        [Header("Car Prefab")]
        public CarManager CarManagerPrefab;

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

    }

}