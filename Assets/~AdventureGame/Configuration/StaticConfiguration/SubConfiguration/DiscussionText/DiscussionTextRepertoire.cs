using UnityEngine;
using UnityEditor;
using OdinSerializer;
using System.Collections.Generic;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTextRepertoire", menuName = "Configuration/AdventureGame/StaticConfiguration/DiscussionTextRepertoire", order = 1)]
    public class DiscussionTextRepertoire : SerializedScriptableObject
    {
        public Dictionary<DisucssionSentenceTextId, string> SentencesText = new Dictionary<DisucssionSentenceTextId, string>();
    }
    
    public enum DisucssionSentenceTextId
    {
        SWER_RTP_1_DOOR = 0
    }
}
