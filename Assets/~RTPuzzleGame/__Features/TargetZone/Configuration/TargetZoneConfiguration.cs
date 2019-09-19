using ConfigurationEditor;
using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TargetZoneConfiguration", menuName = "Configuration/PuzzleGame/TargetZoneConfiguration/TargetZonesConfiguration", order = 1)]
    public class TargetZoneConfiguration : ConfigurationSerialization<TargetZoneID, TargetZoneInherentData>
    { }

}
