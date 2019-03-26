using UnityEngine;
using System.Collections;
using ConfigurationEditor;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionConfiguration", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/PlayerActionConfiguration", order = 1)]
    public class PlayerActionConfiguration : ConfigurationSerialization<PlayerActionId, PlayerActionInherentData>
    {
        
    }

}
