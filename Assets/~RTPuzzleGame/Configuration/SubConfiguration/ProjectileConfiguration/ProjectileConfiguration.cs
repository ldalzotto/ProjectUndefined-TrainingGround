using ConfigurationEditor;
using CoreGame;
using GameConfigurationID;
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

}
