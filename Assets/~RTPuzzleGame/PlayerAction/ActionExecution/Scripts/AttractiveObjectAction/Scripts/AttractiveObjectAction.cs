using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectAction : RTPPlayerAction
    {
        #region External Dependencies
        private AttractiveObjectsInstanciatedParent AttractiveObjectsInstanciatedParent;
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
        private RangeTypeObject attractiveObjectRange;
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
            this.attractiveObjectId = attractiveObjectActionInherentData.AttractiveObjectId;

            #region External Dependencies
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            this.AttractiveObjectsInstanciatedParent = GameObject.FindObjectOfType<AttractiveObjectsInstanciatedParent>();
            var playerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            this.PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var puzzleStaticConfiguration = GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>().PuzzleStaticConfiguration;
            var animationConfiguration = GameObject.FindObjectOfType<CoreConfigurationManager>().AnimationConfiguration();
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            #endregion

            var attractiveObjectInherentConfigurationData = PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[this.attractiveObjectId];
            var attractiveObjectDefinition = this.PuzzleGameConfigurationManager.InteractiveObjectTypeDefinitionConfiguration()[attractiveObjectInherentConfigurationData.AttractiveInteractiveObjectDefinition];

            this.attractiveObject =
                AttractiveObjectActionInstanceHelper.CreateAttractiveObjectAtStart(attractiveObjectDefinition, puzzleStaticConfiguration.PuzzlePrefabConfiguration, PuzzleGameConfigurationManager.PuzzleGameConfiguration,
                        this.AttractiveObjectsInstanciatedParent.transform);

            this.AttractiveObjectInputManager = new AttractiveObjectInputManager(gameInputManager);
            this.AttractiveObjectGroundPositioner = new AttractiveObjectGroundPositioner(playerDataRetriever.GetPlayerRigidBody(), playerDataRetriever.GetPlayerPuzzleLogicRootCollier());
            this.AttractiveObjectPlayerAnimationManager = new AttractiveObjectPlayerAnimationManager(playerDataRetriever, attractiveObjectInherentConfigurationData, this, animationConfiguration, this.attractiveObject);

            this.attractiveObjectRange = RangeTypeObject.InstanciateSphereRange(RangeTypeID.ATTRACTIVE_OBJECT, attractiveObjectInherentConfigurationData.EffectRange, playerDataRetriever.GetPlayerWorldPosition);
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
                this.PuzzleEventsManager.PZ_EVT_AttractiveObject_OnPlayerActionExecuted(objectSpawnPosition.Value, this.attractiveObject, this.PuzzleGameConfigurationManager, this.AttractiveObjectsInstanciatedParent);
            }

            this.OnEndAction();
            this.PlayerActionConsumed();
        }
        private void OnEndAction()
        {
            this.AttractiveObjectPlayerAnimationManager.OnAttractiveObjectActionEnd();
            this.attractiveObjectRange.OnRangeDestroyed();
            this.isActionOver = true;
        }
        #endregion
    }

    class AttractiveObjectPlayerAnimationManager
    {
        private PlayerAnimationWithObjectManager PlayerPocketObjectOutAnimationManager;
        private PlayerAnimationWithObjectManager PlayerObjectLayAnimationManager;

        private bool isOkInputAnimationRunning = false;

        public bool IsOkInputAnimationRunning { get => isOkInputAnimationRunning; }

        public AttractiveObjectPlayerAnimationManager(PlayerManagerDataRetriever playerManagerDataRetriever, AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData,
                AttractiveObjectAction attractiveObjectActionRef, AnimationConfiguration animationConfiguration, InteractiveObjectType attractiveObjectRef)
        {
            this.PlayerPocketObjectOutAnimationManager =
                new PlayerAnimationWithObjectManager(attractiveObjectRef.gameObject, animationConfiguration, attractiveObjectInherentConfigurationData.PreActionAnimation, playerManagerDataRetriever.GetPlayerAnimator(), 0f, false, null);
            this.PlayerObjectLayAnimationManager =
                 new PlayerAnimationWithObjectManager(attractiveObjectRef.gameObject, animationConfiguration, attractiveObjectInherentConfigurationData.PostActionAnimation, playerManagerDataRetriever.GetPlayerAnimator(), 0.05f, false, () => { attractiveObjectActionRef.OnObjectAnimationlayed(); });
            this.PlayerPocketObjectOutAnimationManager.Play();
            this.Tick(0);
        }

        public void Tick(float d)
        {
            this.PlayerPocketObjectOutAnimationManager.Tick(d);
            this.PlayerObjectLayAnimationManager.Tick(d);
        }

        public void OnAttractiveObjectActionEnd()
        {
            this.PlayerPocketObjectOutAnimationManager.Kill();
        }

        public void OnAttractiveObjectActionOKInput()
        {
            this.isOkInputAnimationRunning = true;
            this.PlayerPocketObjectOutAnimationManager.Kill();
            this.PlayerObjectLayAnimationManager.Play();
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
