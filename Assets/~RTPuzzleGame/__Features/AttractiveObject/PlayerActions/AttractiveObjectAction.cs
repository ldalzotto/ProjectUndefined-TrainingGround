using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;
using static AnimationConstants;

namespace RTPuzzle
{
    public class AttractiveObjectAction : RTPPlayerAction
    {
        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private PuzzleEventsManager PuzzleEventsManager;
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion

        #region Internal Managers
        private AttractiveObjectInputManager AttractiveObjectInputManager;
        private AttractiveObjectGroundPositioner AttractiveObjectGroundPositioner;
        private AttractiveObjectPlayerAnimationManager AttractiveObjectPlayerAnimationManager;
        #endregion

        private bool isActionOver;
        private AttractiveObjectId attractiveObjectId;
        private InteractiveObjectType attractiveObjectRangeInteractiveObject;
        private InteractiveObjectType attractiveObject;

        public AttractiveObjectAction(AttractiveObjectActionInherentData attractiveObjectActionInherentData) : base(attractiveObjectActionInherentData)
        {
        }

        public override bool FinishedCondition()
        {
            return isActionOver;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            this.isActionOver = false;
            var attractiveObjectActionInherentData = ((AttractiveObjectActionInherentData)this.playerActionInherentData);

            #region External Dependencies
            var gameInputManager = CoreGameSingletonInstances.GameInputManager;
            var playerDataRetriever = PuzzleGameSingletonInstances.PlayerManagerDataRetriever;
            this.PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var puzzleStaticConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration;
            var animationConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.AnimationConfiguration();
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            #endregion

            this.attractiveObjectId = this.PuzzleGameConfigurationManager.InteractiveObjectTypeDefinitionConfiguration()[attractiveObjectActionInherentData.AttractiveObjectDefinitionID].GetDefinitionModule<AttractiveObjectModuleDefinition>().AttractiveObjectId;

            var attractiveObjectInherentConfigurationData = PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[this.attractiveObjectId];
            var attractiveObjectDefinition = this.PuzzleGameConfigurationManager.InteractiveObjectTypeDefinitionConfiguration()[attractiveObjectActionInherentData.AttractiveObjectDefinitionID];

            this.attractiveObject =
                AttractiveObjectActionInstanceHelper.CreateAttractiveObjectAtStart(attractiveObjectDefinition, puzzleStaticConfiguration.PuzzlePrefabConfiguration, PuzzleGameConfigurationManager.PuzzleGameConfiguration,
                        this.InteractiveObjectContainer.transform);

            this.AttractiveObjectInputManager = new AttractiveObjectInputManager(gameInputManager);
            this.AttractiveObjectGroundPositioner = new AttractiveObjectGroundPositioner(playerDataRetriever.GetPlayerRigidBody(), playerDataRetriever.GetPlayerPuzzleLogicRootCollier());
            this.AttractiveObjectPlayerAnimationManager = new AttractiveObjectPlayerAnimationManager(playerDataRetriever, attractiveObjectInherentConfigurationData, this, this.attractiveObject, this.PuzzleGameConfigurationManager.PuzzleGameConfiguration.PuzzleCutsceneConfiguration,
                 this.InteractiveObjectContainer);

            var attractiveObjectRangeInteractiveObjectInherentData =
                InRangeVisualFeedbackModuleInstancer.BuildInRangeVisualFeedbackFromRange(RangeTypeObjectDefinitionConfigurationInherentDataBuilder.SphereRangeWithObstacleListener(attractiveObjectInherentConfigurationData.EffectRange, RangeTypeID.ATTRACTIVE_OBJECT, withRangeColliderTracker: true));
            this.attractiveObjectRangeInteractiveObject =
                InteractiveObjectType.Instantiate(attractiveObjectRangeInteractiveObjectInherentData, new InteractiveObjectInitializationObject(), puzzleStaticConfiguration.PuzzlePrefabConfiguration, this.PuzzleGameConfigurationManager.PuzzleGameConfiguration);
            this.attractiveObjectRangeInteractiveObject.transform.position = playerDataRetriever.GetPlayerWorldPosition();
        }

        public override void GizmoTick()
        {

        }

        public override void GUITick()
        {

        }

        public override void Tick(float d)
        {
            this.AttractiveObjectPlayerAnimationManager.Tick(d);

            if (!this.AttractiveObjectPlayerAnimationManager.IsOkInputAnimationRunning)
            {
                this.AttractiveObjectInputManager.Tick();
                if (this.AttractiveObjectInputManager.ActionButtonPressed)
                {
                    this.AttractiveObjectPlayerAnimationManager.OnAttractiveObjectActionOKInput();
                }
                if (this.AttractiveObjectInputManager.ReturnButtonPressed)
                {
                    this.InteractiveObjectContainer.OnInteractiveObjectDestroyed(this.attractiveObject);
                    this.OnEndAction();
                }
            }
        }

        public override void LateTick(float d)
        {
        }

        #region Internal Events
        public void OnObjectAnimationlayed()
        {
            var objectSpawnPosition = this.AttractiveObjectGroundPositioner.GetAttractiveObjectSpawnPosition();
            if (objectSpawnPosition.HasValue)
            {
                this.attractiveObject.EnableAllDisabledModules(new InteractiveObjectInitializationObject());
                ((IInteractiveObjectTypeDataRetrieval)this.attractiveObject).GetIAttractiveObjectModuleDataRetriever()
                        .GetIAttractiveObjectModuleEvent().OnAttractiveObjectPlayerActionExecuted(objectSpawnPosition.Value);
            }

            this.OnEndAction();
            this.PlayerActionConsumed();
        }
        private void OnEndAction()
        {
            this.AttractiveObjectPlayerAnimationManager.OnAttractiveObjectActionEnd();
            this.InteractiveObjectContainer.OnInteractiveObjectDestroyed(this.attractiveObjectRangeInteractiveObject);
            this.isActionOver = true;
        }
        #endregion
    }

    class AttractiveObjectPlayerAnimationManager
    {
        private SequencedActionPlayer PocketObjectOutAnimationPlayer;
        private SequencedActionPlayer PocketObjectLayAnimationPlayer;

        private bool isOkInputAnimationRunning = false;

        public bool IsOkInputAnimationRunning { get => isOkInputAnimationRunning; }

        public AttractiveObjectPlayerAnimationManager(PlayerManagerDataRetriever playerManagerDataRetriever, AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData,
                AttractiveObjectAction attractiveObjectActionRef, InteractiveObjectType attractiveObjectRef, PuzzleCutsceneConfiguration PuzzleCutsceneConfiguration, InteractiveObjectContainer InteractiveObjectContainer)
        {
            this.PocketObjectOutAnimationPlayer = new SequencedActionPlayer(PuzzleCutsceneConfiguration.ConfigurationInherentData[attractiveObjectInherentConfigurationData.PreActionAnimationGraph].PuzzleCutsceneGraph.GetRootActions(),
                        new PuzzleCutsceneActionInput(InteractiveObjectContainer,
                            PuzzleCutsceneActionInput.Build_GENERIC_AnimationWithFollowObject_Animation(BipedBoneRetriever.GetPlayerBone(BipedBone.RIGHT_HAND_CONTEXT, playerManagerDataRetriever.GetPlayerAnimator()).transform,
                               attractiveObjectRef.gameObject, attractiveObjectInherentConfigurationData.PreActionAnimation)));

            this.PocketObjectLayAnimationPlayer = new SequencedActionPlayer(PuzzleCutsceneConfiguration.ConfigurationInherentData[attractiveObjectInherentConfigurationData.PostActionAnimationGraph].PuzzleCutsceneGraph.GetRootActions(),
              new PuzzleCutsceneActionInput(InteractiveObjectContainer,
                  PuzzleCutsceneActionInput.Build_GENERIC_AnimationWithFollowObject_Animation(BipedBoneRetriever.GetPlayerBone(BipedBone.RIGHT_HAND_CONTEXT, playerManagerDataRetriever.GetPlayerAnimator()).transform,
                     attractiveObjectRef.gameObject, attractiveObjectInherentConfigurationData.PostActionAnimation)),
                    onFinished: attractiveObjectActionRef.OnObjectAnimationlayed);

            this.PocketObjectOutAnimationPlayer.Play();
            this.Tick(0);
        }

        public void Tick(float d)
        {
            this.PocketObjectOutAnimationPlayer.Tick(d);
            this.PocketObjectLayAnimationPlayer.Tick(d);
        }

        public void OnAttractiveObjectActionEnd()
        {
            this.PocketObjectOutAnimationPlayer.Kill();
        }

        public void OnAttractiveObjectActionOKInput()
        {
            this.isOkInputAnimationRunning = true;
            this.PocketObjectOutAnimationPlayer.Kill();
            this.PocketObjectLayAnimationPlayer.Play();
        }

    }

    class AttractiveObjectInputManager
    {

        private GameInputManager GameInputManager;

        private bool actionButtonPressed;
        private bool returnButtonPressed;

        public AttractiveObjectInputManager(GameInputManager gameInputManager)
        {
            GameInputManager = gameInputManager;
        }

        public bool ActionButtonPressed { get => actionButtonPressed; }
        public bool ReturnButtonPressed { get => returnButtonPressed; }

        public void Tick()
        {
            this.actionButtonPressed = this.GameInputManager.CurrentInput.ActionButtonD();
            this.returnButtonPressed = this.GameInputManager.CurrentInput.CancelButtonD();
        }

    }

    class AttractiveObjectGroundPositioner
    {
        private Rigidbody playerRigidBody;
        private Collider playerCollider;

        public AttractiveObjectGroundPositioner(Rigidbody playerRigidBody, Collider playerCollider)
        {
            this.playerRigidBody = playerRigidBody;
            this.playerCollider = playerCollider;
        }

        public Nullable<RaycastHit> GetAttractiveObjectSpawnPosition()
        {
            RaycastHit hit;
            if (PhysicsHelper.RaycastToDownVertically(playerCollider, playerRigidBody, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER), out hit))
            {
                return hit;
            }
            else
            {
                return null;
            }
        }

    }

    public static class AttractiveObjectActionInstanceHelper
    {
        public static InteractiveObjectType CreateAttractiveObjectAtStart(InteractiveObjectTypeDefinitionInherentData InteractiveObjectTypeDefinitionInherentData,
                        PuzzlePrefabConfiguration puzzlePrefabConfiguration, PuzzleGameConfiguration puzzleGameConfiguration,
                        Transform parent)
        {
            return InteractiveObjectType.Instantiate(InteractiveObjectTypeDefinitionInherentData, new InteractiveObjectInitializationObject(), puzzlePrefabConfiguration, puzzleGameConfiguration, parent,
                new List<Type>() { typeof(ModelObjectModule) });
        }
    }
}
