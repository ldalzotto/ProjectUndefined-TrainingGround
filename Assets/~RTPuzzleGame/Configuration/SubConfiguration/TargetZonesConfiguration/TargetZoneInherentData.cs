using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TargetZoneInherentData", menuName = "Configuration/PuzzleGame/TargetZonesConfiguration/TargetZoneInherentData", order = 1)]
    public class TargetZoneInherentData : ScriptableObject
    {
        [SerializeField]
        private float escapeMinDistance;

        [SerializeField]
        private float escapeFOVSemiAngle;

        public float EscapeMinDistance { get => escapeMinDistance; set => escapeMinDistance = value; }
        public float EscapeFOVSemiAngle { get => escapeFOVSemiAngle; set => escapeFOVSemiAngle = value; }
    }

}
