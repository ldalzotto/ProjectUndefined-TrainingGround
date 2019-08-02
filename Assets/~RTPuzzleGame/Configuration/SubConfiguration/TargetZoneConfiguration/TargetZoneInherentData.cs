using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TargetZoneInherentData", menuName = "Configuration/PuzzleGame/TargetZoneConfiguration/TargetZoneInherentData", order = 1)]
    public class TargetZoneInherentData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The distance from which the AI will take knowledge of the target zone.")]
        private float aiDistanceDetection;

        [SerializeField]
        private float escapeFOVSemiAngle;

        [SerializeField]
        public InteractiveObjectType AssociatedInteractiveObjectType;

        public TargetZoneInherentData(float aiDistanceDetection, float escapeFOVSemiAngle)
        {
            this.aiDistanceDetection = aiDistanceDetection;
            this.escapeFOVSemiAngle = escapeFOVSemiAngle;
        }

        public float AIDistanceDetection { get => aiDistanceDetection; set => aiDistanceDetection = value; }
        public float EscapeFOVSemiAngle { get => escapeFOVSemiAngle; set => escapeFOVSemiAngle = value; }
    }

}
