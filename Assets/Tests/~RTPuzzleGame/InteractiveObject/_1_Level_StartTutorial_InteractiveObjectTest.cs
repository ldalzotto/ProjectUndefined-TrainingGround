using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections;
using System.Collections.Generic;
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

            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();


            var projectileThrowRange = 9993f;
            var projectileEffectRange = 9994f;
            var projectile = ProjectileInteractiveObjectDefinitions._1_Town_Speaker(InteractiveObjectTestID.TEST_1,
                 explodingEffectRange: 9991f, travelDistancePerSeconds: 9992f, projectileThrowRange: projectileThrowRange,
                 attractiveObjectEffectRange: projectileEffectRange, attractiveObjectEffectiveTime: 9995f,
                 grabObjectRadius: 1.5f,
                 disarmInteractionRange: 2.5f, disarmTime: 2f
            );

            var launchProjetileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(
                 projectile.InteractiveObjectTypeDefinitionID,
                 SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f)
            );

            playerActionManager.ExecuteAction(launchProjetileAction);
            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(projectile.InteractiveObjectID);

            Assert.IsTrue(interactiveLaunchProjectiles.Count == 1);
            var interactiveProjectile = interactiveLaunchProjectiles[0];

            var enabledModules = interactiveProjectile.GetAllEnabledModules();
            Assert.IsTrue(enabledModules.Count == 1);
            Assert.IsTrue(enabledModules[0].GetType() == typeof(ModelObjectModule));

            //We check that projectile throw range is equal to the configuration
            Assert.AreEqual(projectileThrowRange, launchProjetileAction.ProjectileThrowRange.RangeType.GetRadiusRange());

            //Because the LaunchProjectileID._1_Town_StartTutorial_Test_Speaker is set to transform to attractive -> projectile effect range is supposed to be the future attractive object range
            var attractiveObjectModule = interactiveProjectile.GetDisabledModule<AttractiveObjectModule>();
            Assert.IsTrue(attractiveObjectModule != null);
            Assert.AreEqual(projectileEffectRange, launchProjetileAction.GetProjectileEffectCursorRange().RangeType.GetRadiusRange());
        }

        [UnityTest]
        public IEnumerator LaunchProjectileAction_Launched_ProjectileModuleEnabled()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest);
            yield return new WaitForFixedUpdate();

            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var projectile = ProjectileInteractiveObjectDefinitions._1_Town_Speaker(InteractiveObjectTestID.TEST_1,
                 explodingEffectRange: 9999f, travelDistancePerSeconds: 9999f, projectileThrowRange: 9999f,
                 attractiveObjectEffectRange: 9999f, attractiveObjectEffectiveTime: 9995f,
                 grabObjectRadius: 1.5f,
                 disarmInteractionRange: 2.5f, disarmTime: 2f
            );
            var launchProjectileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(
                    projectile.InteractiveObjectTypeDefinitionID,
                    SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjectileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjectileAction);
            launchProjectileAction.SpawnLaunchProjectile(Vector3.zero);

            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(projectile.InteractiveObjectID);

            Assert.AreEqual(1, interactiveLaunchProjectiles.Count);

            var interactiveProjectile = interactiveLaunchProjectiles[0];

            Assert.AreEqual(2, interactiveProjectile.GetAllEnabledModules().Count);
            this.InteractiveObjectModulePresenceAssert(interactiveProjectile, enabledModulesToCheck: new List<Type>() { typeof(ModelObjectModule), typeof(LaunchProjectileModule) });

        }

        [UnityTest]
        public IEnumerator LaunchProjectileModule_ReachedDestination_IfIsPersistingToAttractiveObject_AttractiveModuleEnabled()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();

            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var aiContainer = GameObject.FindObjectOfType<AIManagerContainer>();

            var projectileThrowRange = 9993f;
            var projectileEffectRange = 9994f;
            var projectile = ProjectileInteractiveObjectDefinitions._1_Town_Speaker(InteractiveObjectTestID.TEST_1,
                 explodingEffectRange: 9991f, travelDistancePerSeconds: 9992f, projectileThrowRange: projectileThrowRange,
                 attractiveObjectEffectRange: projectileEffectRange, attractiveObjectEffectiveTime: 9995f,
                 grabObjectRadius: 1.5f,
                 disarmInteractionRange: 2.5f, disarmTime: 2f
            );
            var launchProjetileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(
                    projectile.InteractiveObjectTypeDefinitionID,
                SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjetileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjetileAction);
            launchProjetileAction.SpawnLaunchProjectile(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PROJECTILE_TARGET_1).transform.position);

            //Wait for projectile to read target
            yield return null;

            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(projectile.InteractiveObjectID);

            Assert.AreEqual(1, interactiveLaunchProjectiles.Count);

            var interactiveProjectile = interactiveLaunchProjectiles[0];

            Assert.AreEqual(4, interactiveProjectile.GetAllEnabledModules().Count);
            this.InteractiveObjectModulePresenceAssert(interactiveProjectile, enabledModulesToCheck: new List<Type>() { typeof(ModelObjectModule), typeof(AttractiveObjectModule), typeof(DisarmObjectModule), typeof(GrabObjectModule) });

            Assert.AreEqual(projectileEffectRange, interactiveProjectile.GetModule<AttractiveObjectModule>().SphereRange.RangeType.GetRadiusRange());
        }

        [UnityTest]
        public IEnumerator DisarmObjectModule_WhenAIIsNear_WithAIDisarmObjectManager_IsDisarming()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();

            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var aiContainer = GameObject.FindObjectOfType<AIManagerContainer>();

            var projectileThrowRange = 9993f;
            var projectileEffectRange = 9994f;
            var projectile = ProjectileInteractiveObjectDefinitions._1_Town_Speaker(InteractiveObjectTestID.TEST_1,
                 explodingEffectRange: 9991f, travelDistancePerSeconds: 9992f, projectileThrowRange: projectileThrowRange,
                 attractiveObjectEffectRange: projectileEffectRange, attractiveObjectEffectiveTime: 9995f,
                 grabObjectRadius: 1.5f,
                 disarmInteractionRange: 2.5f, disarmTime: 2f
            );
            var launchProjetileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(
                    projectile.InteractiveObjectTypeDefinitionID,
                SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjetileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjetileAction);
            launchProjetileAction.SpawnLaunchProjectile(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PROJECTILE_TARGET_1).transform.position);


            //Wait for projectile to read target
            yield return null;
            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(projectile.InteractiveObjectID);
            Assert.AreEqual(1, interactiveLaunchProjectiles.Count);

            var interactiveProjectile = interactiveLaunchProjectiles[0];
            var DisarmObjectModule = interactiveProjectile.GetModule<DisarmObjectModule>();
            var GrabObjectModule = interactiveProjectile.GetModule<GrabObjectModule>();

            var awaitedDisarmObjectRange = puzzleConfigurationManager.DisarmObjectsConfiguration()[DisarmObjectModule.DisarmObjectID].DisarmInteractionRange;
            Assert.AreEqual(awaitedDisarmObjectRange, DisarmObjectModule.GetEffectRange());

            Assert.AreEqual(0f, DisarmObjectModule.GetDisarmPercentage01());
            TestHelperMethods.SetAgentPosition(aiManager.GetAgent(), DisarmObjectModule.transform.position);
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.IsTrue(DisarmObjectModule.GetDisarmPercentage01() > 0f);
        }

        [UnityTest]
        public IEnumerator DisarmObjectModule_WhenAIDisarmObject_GrabModuleIsDisabled()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();

            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();

            var projectileThrowRange = 9993f;
            var projectileEffectRange = 9994f;
            var projectile = ProjectileInteractiveObjectDefinitions._1_Town_Speaker(InteractiveObjectTestID.TEST_1,
                 explodingEffectRange: 9991f, travelDistancePerSeconds: 9992f, projectileThrowRange: projectileThrowRange,
                 attractiveObjectEffectRange: projectileEffectRange, attractiveObjectEffectiveTime: 9995f,
                 grabObjectRadius: 1.5f,
                 disarmInteractionRange: 2.5f, disarmTime: 2f
            );
            var launchProjetileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(
                    projectile.InteractiveObjectTypeDefinitionID,
                SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjetileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjetileAction);
            launchProjetileAction.SpawnLaunchProjectile(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PROJECTILE_TARGET_1).transform.position);

            //Wait for projectile to read target
            yield return null;
            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(projectile.InteractiveObjectID);

            var interactiveProjectile = interactiveLaunchProjectiles[0];
            var DisarmObjectModule = interactiveProjectile.GetModule<DisarmObjectModule>();
            var GrabObjectModule = interactiveProjectile.GetModule<GrabObjectModule>();

            TestHelperMethods.SetAgentPosition(aiManager.GetAgent(), DisarmObjectModule.transform.position);

            Assert.IsTrue(GrabObjectModule.isActiveAndEnabled);

            yield return new WaitForFixedUpdate();
            yield return null;
            //When disarming, the grab module is disabled
            Assert.IsFalse(GrabObjectModule.isActiveAndEnabled);

            var playerObject = interactiveObjectContainer.GetInteractiveObjectFirst(InteractiveObjectID.Player);
            playerObject.transform.position = aiManager.GetAgent().transform.position + (aiManager.GetAgent().transform.forward * 3f);

            yield return new WaitForFixedUpdate();
            yield return null;

            //When disarming disabled, the grab module is re-enabled
            Assert.IsTrue(GrabObjectModule.isActiveAndEnabled);
        }

        [UnityTest]
        public IEnumerator GrabObjectModule_WhenPlayerIsNear_IsAddingAGrabPlayerAction()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest);
            yield return new WaitForFixedUpdate();

            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var aiContainer = GameObject.FindObjectOfType<AIManagerContainer>();
            var InteractiveObjectSelectionManager = GameObject.FindObjectOfType<InteractiveObjectSelectionManager>();


            var projectileThrowRange = 9993f;
            var projectileEffectRange = 9994f;
            var projectile = ProjectileInteractiveObjectDefinitions._1_Town_Speaker(InteractiveObjectTestID.TEST_1,
                 explodingEffectRange: 9991f, travelDistancePerSeconds: 9992f, projectileThrowRange: projectileThrowRange,
                 attractiveObjectEffectRange: projectileEffectRange, attractiveObjectEffectiveTime: 9995f,
                 grabObjectRadius: 1.5f,
                 disarmInteractionRange: 2.5f, disarmTime: 2f
            );
            var launchProjetileAction = new LaunchProjectileAction(new LaunchProjectileActionInherentData(
                    projectile.InteractiveObjectTypeDefinitionID,
                SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, 0f));
            var launchProjectileActionRemainingExecutions = launchProjetileAction.RemainingExecutionAmout;
            playerActionManager.ExecuteAction(launchProjetileAction);
            launchProjetileAction.SpawnLaunchProjectile(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PROJECTILE_TARGET_1).transform.position);

            //Wait for projectile to read target
            yield return null;
            var interactiveLaunchProjectiles = interactiveObjectContainer.GetIntractiveObjectsAll(projectile.InteractiveObjectID);
            Assert.AreEqual(1, interactiveLaunchProjectiles.Count);

            var interactiveProjectile = interactiveLaunchProjectiles[0];
            var grabObjectModule = interactiveProjectile.GetModule<GrabObjectModule>();

            var awaitedGrabObjectModuleEffectRange = puzzleConfigurationManager.GrabObjectConfiguration()[grabObjectModule.GrabObjectID].EffectRadius;
            Assert.AreEqual(awaitedGrabObjectModuleEffectRange, grabObjectModule.GrabObjectRange.radius);

            var PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var initialPlayerActionsCount = PlayerActionManager.GetCurrentAvailablePlayerActions().MultiValueGetValues().Count;

            var PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            PlayerManager.transform.position = grabObjectModule.transform.position;

            //Player action selected presence
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject() == null);
            yield return new WaitForFixedUpdate();
            yield return null; //Wait for player action selection to update current available player actions
            Assert.IsTrue(InteractiveObjectSelectionManager.GetCurrentSelectedObject() != null);
            Assert.IsTrue(PlayerActionManager.GetCurrentAvailablePlayerActions().MultiValueGetValues().Count == initialPlayerActionsCount + 1);

            bool currentAvailablePlayerActionsContainsGrabObjectAction = false;
            bool grabObjectActionReferToGrabObjectConfiguredAction = false;

            foreach (var playerAction in PlayerActionManager.GetCurrentAvailablePlayerActions().MultiValueGetValues())
            {
                if (playerAction.GetType() == typeof(GrabObjectAction))
                {
                    currentAvailablePlayerActionsContainsGrabObjectAction = true;

                    var grabAction = (GrabObjectAction)playerAction;
                    grabObjectActionReferToGrabObjectConfiguredAction = (grabAction.GetPlayerActionToIncrementOrAdd() == puzzleConfigurationManager.GrabObjectConfiguration()[grabObjectModule.GrabObjectID].PlayerActionToIncrementOrAdd);
                }
            }

            Assert.IsTrue(currentAvailablePlayerActionsContainsGrabObjectAction);
            Assert.IsTrue(grabObjectActionReferToGrabObjectConfiguredAction);
        }

        [UnityTest]
        public IEnumerator ActionInteractableObjectModule_WhenPlayerIsNear_IsAddingAssociatedAction()
        {
            var playerActionID = PlayerActionId.TEST_1;
            InteractiveObjectType interactableObject = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                interactableObject = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_1, new ActionInteractableObjectInitialization(
                           9999f, playerActionID, 0f, 1, PuzzleCutsceneID.TEST_1, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                    )).Instanciate(Vector3.zero);
            });
            yield return new WaitForFixedUpdate();

            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();

            var currentAvailablePlayerActions = playerActionManager.GetCurrentAvailablePlayerActions();
            Assert.IsTrue(currentAvailablePlayerActions.Count == 1);
        }

        [UnityTest]
        public IEnumerator ActionInteractableObjectModule_WhenActionIsConsumed_ActionInteractableObjectModuleDisabled()
        {
            var playerActionID = PlayerActionId.TEST_1;
            InteractiveObjectType interactableObject = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_InteractiveObjectTest, () =>
            {
                interactableObject = ActionInteractableObjectDefinition.ActionInteractableObjectOnly(InteractiveObjectTestID.TEST_1, new ActionInteractableObjectInitialization(
                           9999f, playerActionID, 0f, 1, PuzzleCutsceneID.TEST_1, ActionInteractableObjectDefinition.DoNothingCutsceneGraph()
                    )).Instanciate(Vector3.zero);
            });
            yield return new WaitForFixedUpdate();

            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var playerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();

            var currentAvailablePlayerActions = playerActionManager.GetCurrentAvailablePlayerActions();
            playerActionManager.ExecuteAction(currentAvailablePlayerActions[PlayerActionId.NONE][0]);
            yield return null;
            Assert.IsTrue(currentAvailablePlayerActions.Count == 0);
            Assert.IsTrue(interactableObject.GetDisabledModule<ActionInteractableObjectModule>());
        }

        private void InteractiveObjectModulePresenceAssert(InteractiveObjectType interactiveObject, List<Type> enabledModulesToCheck = null, List<Type> disabledModulesToCheck = null)
        {
            if (enabledModulesToCheck != null)
            {
                var enabledModules = interactiveObject.GetAllEnabledModules().ConvertAll(m => m.GetType());
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
