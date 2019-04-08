using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TargetZoneInherentData", menuName = "Configuration/PuzzleGame/TargetZonesConfiguration/TargetZoneInherentData", order = 1)]
    public class TargetZoneInherentData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The distance from which the AI will take knowledge of the target zone.")]
        private float aiDistanceDetection;

        [SerializeField]
        [Tooltip("The total distance crossed by AI when escaping from target zone.")]
        private float escapeDistance;

        [SerializeField]
        private float escapeFOVSemiAngle;

        public TargetZoneInherentData(float aiDistanceDetection, float escapeDistance, float escapeFOVSemiAngle)
        {
            this.aiDistanceDetection = aiDistanceDetection;
            this.escapeDistance = escapeDistance;
            this.escapeFOVSemiAngle = escapeFOVSemiAngle;
        }

        public float AIDistanceDetection { get => aiDistanceDetection; set => aiDistanceDetection = value; }
        public float EscapeFOVSemiAngle { get => escapeFOVSemiAngle; set => escapeFOVSemiAngle = value; }
        public float EscapeDistance { get => escapeDistance; set => escapeDistance = value; }
    }

}
