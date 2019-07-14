using UnityEngine;
using UnityEditor;
using OdinSerializer;
using System.Collections.Generic;
using GameConfigurationID;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTextRepertoire", menuName = "Configuration/AdventureGame/StaticConfiguration/DiscussionTextRepertoire", order = 1)]
    public class DiscussionTextRepertoire : SerializedScriptableObject
    {
        public Dictionary<DisucssionSentenceTextId, string> SentencesText = new Dictionary<DisucssionSentenceTextId, string>();
    }
    
}
