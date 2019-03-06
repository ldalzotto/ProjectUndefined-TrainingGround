using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTPuzzle
{

    public class PuzzleGameConfigurationManager : MonoBehaviour
    {
        public PuzzleGameConfiguration PuzzleGameConfiguration;

        public Dictionary<LaunchProjectileId, ProjectileInherentData> ProjectileConf()
        {
            return PuzzleGameConfiguration.ProjectileConfiguration.LaunchProjectileInherentDatas;
        }
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleGameConfiguration", menuName = "Configuration/PuzzleGame/PuzzleGameConfiguration", order = 1)]
    public class PuzzleGameConfiguration : ScriptableObject
    {
        public LaunchProjectileInherentDataConfiguration ProjectileConfiguration;
    }

}
