using ConfigurationEditor;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DottedLineConfiguration", menuName = "Configuration/PuzzleGame/DottedLineConfiguration/DottedLineConfiguration", order = 1)]
    public class DottedLineConfiguration : ConfigurationSerialization<DottedLineID, DottedLineInherentData>
    { }

    public enum DottedLineID
    {
        PROJECTILE_POSITIONING,
        ATTRACTIVE_OBJECT
    }

}
