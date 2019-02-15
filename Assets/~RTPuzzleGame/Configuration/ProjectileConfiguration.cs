using System.Collections.Generic;

namespace RTPuzzle
{
    public class LaunchProjectileInherentData
    {
        private float effectRange;
        private float escapeSemiAngle;
        private float travelDistanceSpeed;

        public LaunchProjectileInherentData(float effectRange, float escapeSemiAngle, float travelDistanceSpeed)
        {
            this.effectRange = effectRange;
            this.escapeSemiAngle = escapeSemiAngle;
            this.travelDistanceSpeed = travelDistanceSpeed;
        }

        public float EffectRange { get => effectRange; }
        public float EscapeSemiAngle { get => escapeSemiAngle; }
        public float TravelDistanceSpeed { get => travelDistanceSpeed; }
    }

    public enum LaunchProjectileId
    {
        STONE
    }

    public class LaunchProjectileInherentDataConfiguration
    {
        public static Dictionary<LaunchProjectileId, LaunchProjectileInherentData> conf = new Dictionary<LaunchProjectileId, LaunchProjectileInherentData>()
        {
            {LaunchProjectileId.STONE, new LaunchProjectileInherentData(8.230255f, 90f, 1f) }
        };
    }
}
