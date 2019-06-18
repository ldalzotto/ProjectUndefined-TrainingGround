using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionConfiguration", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/PlayerActionConfiguration", order = 1)]
    public class PlayerActionConfiguration : ConfigurationSerialization<PlayerActionId, PlayerActionInherentData>
    {
        
    }

}
