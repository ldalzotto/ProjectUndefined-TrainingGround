using UnityEngine;
using System.Collections;
using ConfigurationEditor;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeTypeConfiguration", menuName = "Configuration/PuzzleGame/RangeTypeConfiguration/RangeTypeConfiguration", order = 1)]
    public class RangeTypeConfiguration : ConfigurationSerialization<RangeTypeID, RangeTypeInherentConfigurationData>
    {

    }
    public enum RangeTypeID
    {
        LAUNCH_PROJECTILE = 0,
        LAUNCH_PROJECTILE_CURSOR = 1,
        ATTRACTIVE_OBJECT = 2,
        ATTRACTIVE_OBJECT_ACTIVE = 3,
        TARGET_ZONE = 4
    }
}
