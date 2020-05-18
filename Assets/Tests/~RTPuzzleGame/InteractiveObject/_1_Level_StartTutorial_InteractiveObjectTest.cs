﻿using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class _1_Level_StartTutorial_InteractiveObjectTest : AbstractPuzzleSceneTest
    {
        [UnityTest]
        public IEnumerator LaunchProjectileModule_Triggered_OnlyModelModuleIsEnabled()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest);
            yield return new WaitForFixedUpdate();

            var projectileID = LaunchProjectileID._1_Town_StartTutorial_Test;
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
            var launchProjectileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(LaunchProjectileID._1_Town_StartTutorial_Test, SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjectileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjectileAction);
            launchProjectileAction.SpawnLaunchProjectile(Vector3.zero);

            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(InteractiveObjectID._1_Town_StartTutorial_Test_Speaker);

            Assert.AreEqual(1, interactiveLaunchProjectiles.Count);

            var interactiveProjectile = interactiveLaunchProjectiles[0];

            Assert.AreEqual(2, interactiveProjectile.GetAllModules().Count);
            this.InteractiveObjectModulePresenceAssert(interactiveProjectile, enabledModulesToCheck: new List<Type>() { typeof(ModelObjectModule), typeof(LaunchProjectileModule) });

        }

        [UnityTest]
        public IEnumerator LaunchProjectileModule_ReachedDestination_IfIsPersistingToAttractiveObject_AttractiveModuleEnabled()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();

            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var aiContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            var aiBehavior = aiContainer.GetNPCAiManager(AiID._1_Town_StartTutorial_AITest);
            var launchProjectileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(LaunchProjectileID._1_Town_StartTutorial_Test, SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjectileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjectileAction);
            launchProjectileAction.SpawnLaunchProjectile(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PROJECTILE_TARGET_1).transform.position);

            //Wait for projectile to read target
            yield return null;

            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(InteractiveObjectID._1_Town_StartTutorial_Test_Speaker);

            Assert.AreEqual(1, interactiveLaunchProjectiles.Count);

            var interactiveProjectile = interactiveLaunchProjectiles[0];

            Assert.AreEqual(4, interactiveProjectile.GetAllModules().Count);
            this.InteractiveObjectModulePresenceAssert(interactiveProjectile, enabledModulesToCheck: new List<Type>() { typeof(ModelObjectModule), typeof(AttractiveObjectModule), typeof(DisarmObjectModule), typeof(GrabObjectModule) });

            var awaitedAttractiveObjectRange = puzzleConfigurationManager.AttractiveObjectsConfiguration()[interactiveProjectile.GetModule<AttractiveObjectModule>().AttractiveObjectId].EffectRange;
            Assert.AreEqual(awaitedAttractiveObjectRange, interactiveProjectile.GetModule<AttractiveObjectModule>().SphereRange.RangeType.GetRadiusRange());
        }

        [UnityTest]
        public IEnumerator DisarmObjectModule_WhenAIIsNear_WithAIDisarmObjectManager_IsDisarming()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();

            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var aiContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            var aiBehavior = aiContainer.GetNPCAiManager(AiID._1_Town_StartTutorial_AITest);
            var launchProjectileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(LaunchProjectileID._1_Town_StartTutorial_Test, SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjectileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjectileAction);
            launchProjectileAction.SpawnLaunchProjectile(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PROJECTILE_TARGET_1).transform.position);

            //Wait for projectile to read target
            yield return null;
            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(InteractiveObjectID._1_Town_StartTutorial_Test_Speaker);
            Assert.AreEqual(1, interactiveLaunchProjectiles.Count);

            var interactiveProjectile = interactiveLaunchProjectiles[0];
            var DisarmObjectModule = interactiveProjectile.GetModule<DisarmObjectModule>();

            var awaitedDisarmObjectRange = puzzleConfigurationManager.DisarmObjectsConfiguration()[DisarmObjectModule.DisarmObjectID].DisarmInteractionRange;
            Assert.AreEqual(awaitedDisarmObjectRange, DisarmObjectModule.GetEffectRange());

            Assert.AreEqual(0f, DisarmObjectModule.GetDisarmPercentage01());
            TestHelperMethods.SetAgentPosition(aiBehavior.GetAgent(), DisarmObjectModule.transform.position);
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.IsTrue(DisarmObjectModule.GetDisarmPercentage01() > 0f);
        }

        [UnityTest]
        public IEnumerator GrabObjectModule_WhenPlayerIsNear_IsAddingAGrabPlayerAction()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();

            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var aiContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            var aiBehavior = aiContainer.GetNPCAiManager(AiID._1_Town_StartTutorial_AITest);
            var launchProjectileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(LaunchProjectileID._1_Town_StartTutorial_Test, SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjectileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjectileAction);
            launchProjectileAction.SpawnLaunchProjectile(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PROJECTILE_TARGET_1).transform.position);

            //Wait for projectile to read target
            yield return null;
            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(InteractiveObjectID._1_Town_StartTutorial_Test_Speaker);
            Assert.AreEqual(1, interactiveLaunchProjectiles.Count);

            var interactiveProjectile = interactiveLaunchProjectiles[0];
            var grabObjectModule = interactiveProjectile.GetModule<GrabObjectModule>();

            var awaitedGrabObjectModuleEffectRange = puzzleConfigurationManager.GrabObjectConfiguration()[grabObjectModule.GrabObjectID].EffectRadius;
            Assert.AreEqual(awaitedGrabObjectModuleEffectRange, grabObjectModule.GrabObjectRange.radius);

            var PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var initialPlayerActionsCount = PlayerActionManager.GetCurrentAvailablePlayerActions().MultiValueGetValues().Count;

            var PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            PlayerManager.transform.position = grabObjectModule.transform.position;

            yield return new WaitForFixedUpdate();

            Assert.IsTrue(PlayerActionManager.GetCurrentAvailablePlayerActions().MultiValueGetValues().Count == initialPlayerActionsCount + 1);

            bool currentAvailablePlayerActionsContainsGrabObjectAction = false;
            bool grabObjectActionReferToGrabObjectConfiguredAction = false;

            foreach(var playerAction in PlayerActionManager.GetCurrentAvailablePlayerActions().MultiValueGetValues())
            {
                if(playerAction.GetType() == typeof(GrabObjectAction))
                {
                    currentAvailablePlayerActionsContainsGrabObjectAction = true;

                    var grabAction = (GrabObjectAction)playerAction;
                    grabObjectActionReferToGrabObjectConfiguredAction = (grabAction.GetPlayerActionToIncrement() == puzzleConfigurationManager.GrabObjectConfiguration()[grabObjectModule.GrabObjectID].PlayerActionToIncrement);
                }
            }

            Assert.IsTrue(currentAvailablePlayerActionsContainsGrabObjectAction);
            Assert.IsTrue(grabObjectActionReferToGrabObjectConfiguredAction);
        }


        private void InteractiveObjectModulePresenceAssert(InteractiveObjectType interactiveObject, List<Type> enabledModulesToCheck = null, List<Type> disabledModulesToCheck = null)
        {
            if (enabledModulesToCheck != null)
            {
                var enabledModules = interactiveObject.GetAllModules().ConvertAll(m => m.GetType());
                foreach (var enabledModule in enabledModulesToCheck)
                {
                    Assert.IsTrue(enabledModules.Contains(enabledModule));
                }
            }

            if (disabledModulesToCheck != null)
            {
                var disabledModules = interactiveObject.GetAllDisabledModules().ConvertAll(m => m.GetType());
                foreach (var diabledModule in disabledModulesToCheck)
                {
                    Assert.IsTrue(disabledModules.Contains(diabledModule));
                }
            }
        }
    }

}
