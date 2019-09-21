using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TargetZoneInherentData", menuName = "Configuration/PuzzleGame/TargetZoneConfiguration/TargetZoneInherentData", order = 1)]
    public class TargetZoneInherentData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The distance from which the AI will take knowledge of the target zone.")]
        [WireCircle(R = 0f, G = 0f, B = 1f)]
        private float aiDistanceDetection;

        [SerializeField]
        [WireArc(R = 0f, G = 1f, B = 0f, Radius = 5f)]
        private float escapeFOVSemiAngle;

        public TargetZoneInherentData(float aiDistanceDetection, float escapeFOVSemiAngle)
        {
            this.aiDistanceDetection = aiDistanceDetection;
            this.escapeFOVSemiAngle = escapeFOVSemiAngle;
        }

        public float AIDistanceDetection { get => aiDistanceDetection; set => aiDistanceDetection = value; }
        public float EscapeFOVSemiAngle { get => escapeFOVSemiAngle; set => escapeFOVSemiAngle = value; }
    }

}
