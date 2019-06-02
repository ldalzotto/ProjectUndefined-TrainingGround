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
        BOUNCER_FORBIDDEN_INTRODUCTION,
        BOUNCER_ASK_AGE,
        BOUNCER_GET_OUT,
        BOUNCER_ALLOWED,
        PLAYER_TELL_AGE,
        PLAYER_AGE_CHOICE_17,
        PLAYER_AGE_CHOICE_18
    }
}
