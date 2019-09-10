using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CorePrefabConfiguration", menuName = "Configuration/CoreGame/CorePrefabConfiguration/CorePrefabConfiguration", order = 1)]
    public class CorePrefabConfiguration : SerializedScriptableObject
    {
        public GameObject ActionWheelNodePrefab;

        [Header("Discussion UI Prefabs")]
        public DiscussionWindow DiscussionUIPrefab;
        public ChoicePopup ChoicePopupPrefab;
        public ChoicePopupText ChoicePopupTextPrefab;
        public InputImageType InputBaseImage;
        public InputImageType LeftMouseBaseImage;
        public InputImageType RightMouseBaseImage;
    }
}
