using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TargetZonesConfiguration", menuName = "Configuration/PuzzleGame/TargetZonesConfiguration/TargetZonesConfiguration", order = 1)]
    public class TargetZonesConfiguration : DictionarySerialization<TargetZoneID, TargetZoneInherentData>
    { }

}
