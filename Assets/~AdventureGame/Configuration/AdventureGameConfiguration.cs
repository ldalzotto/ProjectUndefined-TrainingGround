using UnityEngine;
using System.Collections;
using CoreGame;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AdventureGameConfiguration", menuName = "Configuration/AdventureGame/AdventureGameConfiguration", order = 1)]
    public class AdventureGameConfiguration : GameConfiguration
    {
        public ItemConfiguration ItemConfiguration;
    }

}
