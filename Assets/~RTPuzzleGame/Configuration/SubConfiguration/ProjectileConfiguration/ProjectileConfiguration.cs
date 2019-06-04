using ConfigurationEditor;
using CoreGame;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "ProjectileConfiguration", menuName = "Configuration/PuzzleGame/ProjectileConfiguration/ProjectileConfiguration", order = 1)]
    public class ProjectileConfiguration : ConfigurationSerialization<LaunchProjectileId, ProjectileInherentData>
    {    }


    [System.Serializable]
    public enum LaunchProjectileId
    {
        STONE = 0,
        STONE_1 = 1,
        RTP_PUZZLE_CREATION_TEST = 2,
        SEWER_RTP_2_STONE = 3
    }
}
