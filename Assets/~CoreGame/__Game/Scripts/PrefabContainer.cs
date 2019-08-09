using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class PrefabContainer : MonoBehaviour
    {

        private static PrefabContainer instance;

        public GameObject ActionWheelNodePrefab;
        public GameObject InventoryMenuCellPrefab;
        public GameObject GiveActionMiniaturePrefab;

        [Header("Player FX")]
        public TriggerableEffect PlayerSmokeEffectPrefab;

        [Header("Discussion UI Prefabs")]
        public DiscussionWindow DiscussionUIPrefab;
        public ChoicePopup ChoicePopupPrefab;
        public ChoicePopupText ChoicePopupTextPrefab;

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
