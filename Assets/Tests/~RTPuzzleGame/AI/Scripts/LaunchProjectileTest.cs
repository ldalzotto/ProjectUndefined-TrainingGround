using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    class LaunchProjectileTest : AbstractPuzzleSceneTest
    {
        [UnityTest]
        public IEnumerator LaunchProjectile_WhenProjectileHit_AIFOVIsReducedBasedOnAgentPosition_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            yield return null;
            var projectileSemiAngle = 90f;
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseTestAIManager.GetAIBehavior().AIComponents, projectileSemiAngle);
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 0f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.GetAgent().transform.position + Vector3.back, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            var fov = (mouseTestAIManager.GetAIBehavior() as GenericPuzzleAIBehavior).GetFOV();
            Assert.AreEqual(2, fov.FovSlices.Count);
            Assert.AreEqual(new FOVSlice(360 - projectileSemiAngle, 360), fov.FovSlices[0], "The FOV reduction should exctly be equal to (projectileSemiAngle * 2) on a plane.");
            Assert.AreEqual(new FOVSlice(0, projectileSemiAngle), fov.FovSlices[1], "The FOV reduction should exctly be equal to (projectileSemiAngle * 2) on a plane.");
        }

        [UnityTest] //this test is when projectile is not perfectly aligned with AI
        public IEnumerator LaunchProjectile_WhenProjectileHit_AtLowerProjectileCollisionSphere_AIFOVIsReducedBasedOnAgentPosition_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            yield return null;
            var projectileSemiAngle = 90f;
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseTestAIManager.GetAIBehavior().AIComponents, projectileSemiAngle);
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 0f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.GetAgent().transform.position + Vector3.back, launchProjectileContainerManager);
            lpTest.transform.position = mouseTestAIManager.GetAgent().transform.position + Vector3.back - new Vector3(0, lpTest.GetGroundCollisionTrackingCollider().bounds.min.y / 2, 0);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            var fov = (mouseTestAIManager.GetAIBehavior() as GenericPuzzleAIBehavior).GetFOV();
            Assert.AreEqual(2, fov.FovSlices.Count);
            Assert.AreEqual(new FOVSlice(360 - projectileSemiAngle, 360), fov.FovSlices[0], "The FOV reduction should exctly be equal to (projectileSemiAngle * 2) on a plane.");
            Assert.AreEqual(new FOVSlice(0, projectileSemiAngle), fov.FovSlices[1], "The FOV reduction should exctly be equal to (projectileSemiAngle * 2) on a plane.");
        }

        [UnityTest] //this test is when projectile is not perfectly aligned with AI
        public IEnumerator LaunchProjectile_WhenProjectileHit_AtUpperProjectileCollisionSphere_AIFOVIsReducedBasedOnAgentPosition_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            yield return null;
            var projectileSemiAngle = 90f;
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseTestAIManager.GetAIBehavior().AIComponents, projectileSemiAngle);
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 0f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.GetAgent().transform.position + Vector3.back, launchProjectileContainerManager);
            lpTest.transform.position = mouseTestAIManager.GetAgent().transform.position + Vector3.back + new Vector3(0, lpTest.GetGroundCollisionTrackingCollider().bounds.max.y / 2, 0);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            var fov = (mouseTestAIManager.GetAIBehavior() as GenericPuzzleAIBehavior).GetFOV();
            Assert.AreEqual(2, fov.FovSlices.Count);
            Assert.AreEqual(new FOVSlice(360 - projectileSemiAngle, 360), fov.FovSlices[0], "The FOV reduction should exctly be equal to (projectileSemiAngle * 2) on a plane.");
            Assert.AreEqual(new FOVSlice(0, projectileSemiAngle), fov.FovSlices[1], "The FOV reduction should exctly be equal to (projectileSemiAngle * 2) on a plane.");
        }
    }
}
