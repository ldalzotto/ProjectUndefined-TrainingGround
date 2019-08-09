using UnityEngine;
using UnityEditor;
using OdinSerializer;
using System.Collections.Generic;
using GameConfigurationID;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTextRepertoire", menuName = "Configuration/CoreGame/StaticConfiguration/DiscussionTextRepertoire", order = 1)]
    public class DiscussionTextRepertoire : SerializedScriptableObject
    {
#if UNITY_EDITOR
        public static string EditorTemporaryChosenId;
#endif
        public Dictionary<DisucssionSentenceTextId, string> SentencesText = new Dictionary<DisucssionSentenceTextId, string>();
    }
    
}
