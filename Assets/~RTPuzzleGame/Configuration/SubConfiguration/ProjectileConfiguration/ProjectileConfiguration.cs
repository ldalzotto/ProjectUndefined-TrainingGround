﻿using CoreGame;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "ProjectileConfiguration", menuName = "Configuration/PuzzleGame/ProjectileConfiguration/ProjectileConfiguration", order = 1)]
    public class ProjectileConfiguration : DictionarySerialization<LaunchProjectileId, ProjectileInherentData>
    {    }

    [System.Serializable]
    public enum LaunchProjectileId
    {
        STONE = 0,
        STONE_1 = 1
    }
}
