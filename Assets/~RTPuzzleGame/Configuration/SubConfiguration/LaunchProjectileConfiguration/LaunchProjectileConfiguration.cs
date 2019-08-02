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
    [CreateAssetMenu(fileName = "LaunchProjectileConfiguration", menuName = "Configuration/PuzzleGame/LaunchProjectileConfiguration/LaunchProjectileConfiguration", order = 1)]
    public class LaunchProjectileConfiguration : ConfigurationSerialization<LaunchProjectileID, LaunchProjectileInherentData>
    {    }

}
