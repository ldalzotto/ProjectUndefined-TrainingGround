using GameConfigurationID;
using OdinSerializer;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTextInherentData", menuName = "Configuration/CoreGame/DiscussionTextConfiguration/DiscussionTextInherentData", order = 1)]
    public class DiscussionTextInherentData : SerializedScriptableObject
    {
        public string Text;
        public List<InputParameter> InputParameters;
    }

    [System.Serializable]
    public class InputParameter
    {
        [CustomEnum()]
        public InputID inputID;
    }
}
