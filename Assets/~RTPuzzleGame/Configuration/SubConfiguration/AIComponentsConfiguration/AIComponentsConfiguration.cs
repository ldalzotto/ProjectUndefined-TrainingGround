using ConfigurationEditor;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIComponentsConfiguration", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIComponentsConfiguration", order = 1)]
    public class AIComponentsConfiguration : ConfigurationSerialization<AiID, AIBehaviorInherentData>
    {
    }

}