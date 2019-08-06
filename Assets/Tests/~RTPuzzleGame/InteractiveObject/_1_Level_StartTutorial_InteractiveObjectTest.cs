using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class _1_Level_StartTutorial_InteractiveObjectTest : AbstractPuzzleSceneTest
    {
        [UnityTest]
        public IEnumerator LaunchProjectileAction_Triggered_OnlyModelModuleIsEnabled()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest);
            yield return new WaitForFixedUpdate();
            var projectileID = LaunchProjectileID._1_Town_StartTutorial_Test_Speaker;
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();

            var launchProjetileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(projectileID, SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            playerActionManager.ExecuteAction(launchProjetileAction);
            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(InteractiveObjectID._1_Town_StartTutorial_Test_Speaker);

            Assert.IsTrue(interactiveLaunchProjectiles.Count == 1);
            var interactiveProjectile = interactiveLaunchProjectiles[0];

            var enabledModules = interactiveProjectile.GetAllModules();
            Assert.IsTrue(enabledModules.Count == 1);
            Assert.IsTrue(enabledModules[0].GetType() == typeof(ModelObjectModule));

            //We check that projectile throw range is equal to the configuration
            var awaitedProjectileThrowRange = puzzleConfigurationManager.ProjectileConf()[projectileID].ProjectileThrowRange;
            Assert.AreEqual(awaitedProjectileThrowRange, launchProjetileAction.ProjectileThrowRange.RangeType.GetRadiusRange());

            //Because the LaunchProjectileID._1_Town_StartTutorial_Test_Speaker is set to transform to attractive -> projectile effect range is supposed to be the future attractive object range
            var attractiveObjectModule = interactiveProjectile.GetDisabledModule<AttractiveObjectModule>();
            Assert.IsTrue(attractiveObjectModule != null);
            var awaitedProjectileCursorRange = puzzleConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectModule.AttractiveObjectId].EffectRange;
            Assert.AreEqual(awaitedProjectileCursorRange, launchProjetileAction.GetProjectileEffectCursorRange().RangeType.GetRadiusRange());
        }

        [UnityTest]
        public IEnumerator LaunchProjectileAction_Launched_ProjectileModuleEnabled()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest);
            yield return new WaitForFixedUpdate();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var launchProjectileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(LaunchProjectileID._1_Town_StartTutorial_Test_Speaker, SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjectileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjectileAction);
            launchProjectileAction.SpawnLaunchProjectile(Vector3.zero);
            yield return new WaitUntil(() =>
                     {
                         Debug.Log(launchProjectileAction.RemainingExecutionAmout);
                         return launchProjectileAction.RemainingExecutionAmout + 1 == launchProjectileActionRemainingExecutions;
                     }
            );

            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(InteractiveObjectID._1_Town_StartTutorial_Test_Speaker);

            Assert.IsTrue(interactiveLaunchProjectiles.Count == 1);
            var interactiveProjectile = interactiveLaunchProjectiles[0];

            var enabledModules = interactiveProjectile.GetAllModules();
            Assert.IsTrue(enabledModules.Count == 2);
            // Assert.IsTrue(enabledModules[0].GetType() == typeof(ModelObjectModule));
        }
    }

}
