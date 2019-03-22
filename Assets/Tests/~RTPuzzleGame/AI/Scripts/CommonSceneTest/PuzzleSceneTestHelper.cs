using RTPuzzle;
using System.Linq;
using UnityEngine;

namespace Tests
{
    class PuzzleSceneTestHelper
    {
        public static LaunchProjectile SpawnProjectile(ProjectileInherentData projectileInherentData, AITestPositionID projectilePoistion, LaunchProjectileContainerManager launchProjectileContainerManager)
        {
            var projectilePosition = GameObject.FindObjectsOfType<AITestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == projectilePoistion).First().transform.position;
            return SpawnProjectile(projectileInherentData, projectilePosition, launchProjectileContainerManager);
        }

        public static LaunchProjectile SpawnProjectile(ProjectileInherentData projectileInherentData, Vector3 projectilePoistion, LaunchProjectileContainerManager launchProjectileContainerManager)
        {
            var launchProjectile = LaunchProjectile.Instantiate(projectileInherentData, new BeziersControlPoints(), launchProjectileContainerManager.transform);
            launchProjectile.transform.position = projectilePoistion;
            return launchProjectile;
        }

        public static AttractiveObjectType SpawnAttractiveObject(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, AITestPositionID aITestPositionID)
        {
            var attractiveObjectSpawnPosition = GameObject.FindObjectsOfType<AITestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == aITestPositionID).First().transform.position;
            return AttractiveObjectType.Instanciate(attractiveObjectSpawnPosition, null, attractiveObjectInherentConfigurationData);
        }

        public static TargetZone FindTargetZone(TargetZoneID targetZoneID)
        {
            return GameObject.FindObjectsOfType<TargetZone>().ToArray().Select(t => t).Where(targetZone => targetZone.TargetZoneID == targetZoneID).First();
        }
    }
}
