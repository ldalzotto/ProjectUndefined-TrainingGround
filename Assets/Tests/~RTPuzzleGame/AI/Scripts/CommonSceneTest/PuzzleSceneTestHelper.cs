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

        public static Transform FindAITestPosition(AITestPositionID aITestPositionID)
        {
            return GameObject.FindObjectsOfType<AITestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == aITestPositionID).First().transform;
        }

        public static AttractiveObjectType SpawnAttractiveObject(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, AITestPositionID aITestPositionID)
        {
            var attractiveObjectSpawnPosition = FindAITestPosition(aITestPositionID).position;
            return AttractiveObjectType.Instanciate(attractiveObjectSpawnPosition, null, attractiveObjectInherentConfigurationData);
        }

        public static TargetZone FindTargetZone(TargetZoneID targetZoneID)
        {
            return GameObject.FindObjectsOfType<TargetZone>().ToArray().Select(t => t).Where(targetZone => targetZone.TargetZoneID == targetZoneID).First();
        }

        public static AttractiveObjectInherentConfigurationData CreateAttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime)
        {
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            var randomAttractiveObjectInherentData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().AttractiveObjectsConfiguration()[AttractiveObjectId.CHEESE];
            attractiveObjectInherentConfigurationData.Init(effectRange, effectiveTime, randomAttractiveObjectInherentData.AttractiveObjectModelPrefab, randomAttractiveObjectInherentData.AttractiveObjectPrefab, randomAttractiveObjectInherentData.AttractiveObjectAIMarkPrefab);
            return attractiveObjectInherentConfigurationData;
        }

        public static ProjectileInherentData CreateProjectileInherentData(float effectRange, float escapeSemiAngle, float travelDistancePerSeconds)
        {
            var projectileData = ScriptableObject.CreateInstance<ProjectileInherentData>();
            var randomProjectileInherentConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().ProjectileConf()[LaunchProjectileId.STONE];
            projectileData.Init(effectRange, escapeSemiAngle, travelDistancePerSeconds, randomProjectileInherentConfiguration.ProjectilePrefab);
            return projectileData;
        }

        public static void InitializeAIComponents(AbstractAIComponents abstractAIComponents)
        {
            if (abstractAIComponents.GetType() == typeof(GenericPuzzleAIComponents))
            {
                var genericPuzzleAIComponents = (GenericPuzzleAIComponents)abstractAIComponents;

                genericPuzzleAIComponents.AIRandomPatrolComponent.MaxDistance = 15f;

                genericPuzzleAIComponents.AIProjectileEscapeComponent.EscapeDistance = 25f;
                
                genericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance = 50f;

                genericPuzzleAIComponents.AIFearStunComponent.FOVSumThreshold = 20f;
                genericPuzzleAIComponents.AIFearStunComponent.TimeWhileBeginFeared = 2f;
            }
        }
    }
}
