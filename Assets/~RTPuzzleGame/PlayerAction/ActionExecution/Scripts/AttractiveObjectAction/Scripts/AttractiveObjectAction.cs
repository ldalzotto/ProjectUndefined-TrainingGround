using CoreGame;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectAction : RTPPlayerAction
    {
        #region External Dependencies
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;
        #endregion

        #region Internal Managers
        private AttractiveObjectInputManager AttractiveObjectInputManager;
        private AttractiveObjectGroundPositioner AttractiveObjectGroundPositioner;
        private AttractiveObjectPlayerAnimationManager AttractiveObjectPlayerAnimationManager;
        #endregion

        private bool isActionOver;
        private AttractiveObjectId attractiveObjectId;
        private SphereRangeType attractiveObjectRange;

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
            this.attractiveObjectId = ((AttractiveObjectActionInherentData)this.playerActionInherentData).AttractiveObjectId;

            #region External Dependencies
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            this.AttractiveObjectsContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
            var playerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            #endregion

            var attractiveObjectInherentConfigurationData = puzzleConfigurationManager.AttractiveObjectsConfiguration()[this.attractiveObjectId];

            this.AttractiveObjectInputManager = new AttractiveObjectInputManager(gameInputManager);
            this.AttractiveObjectGroundPositioner = new AttractiveObjectGroundPositioner(playerDataRetriever.GetPlayerRigidBody(), playerDataRetriever.GetPlayerCollider());
            this.AttractiveObjectPlayerAnimationManager = new AttractiveObjectPlayerAnimationManager(playerDataRetriever, attractiveObjectInherentConfigurationData, this);

            this.attractiveObjectRange = SphereRangeType.Instanciate(RangeTypeID.ATTRACTIVE_OBJECT, attractiveObjectInherentConfigurationData.EffectRange, playerDataRetriever.GetPlayerWorldPosition);
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
            this.AttractiveObjectInputManager.Tick();
            if (this.AttractiveObjectInputManager.ActionButtonPressed)
            {
                this.AttractiveObjectPlayerAnimationManager.OnAttractiveObjectActionOKInput();
            }
            if (this.AttractiveObjectInputManager.ReturnButtonPressed)
            {
                this.OnEndAction();
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
                this.AttractiveObjectsContainerManager.OnAttractiveObjectActionExecuted(objectSpawnPosition.Value, ((AttractiveObjectActionInherentData)this.playerActionInherentData).AttractiveObjectId);
            }

            this.OnEndAction();
            ResetCoolDown();
        }
        private void OnEndAction()
        {
            this.AttractiveObjectPlayerAnimationManager.OnAttractiveObjectActionEnd();
            MonoBehaviour.Destroy(this.attractiveObjectRange.gameObject);
            this.isActionOver = true;
        }
        #endregion
    }

    class AttractiveObjectPlayerAnimationManager
    {
        private PlayerAnimationWithObjectManager PlayerPocketObjectOutAnimationManager;
        private PlayerAnimationWithObjectManager PlayerObjectLayAnimationManager;
        private GameObject attractiveObjectInstance;

        public AttractiveObjectPlayerAnimationManager(PlayerManagerDataRetriever playerManagerDataRetriever, AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, AttractiveObjectAction attractiveObjectActionRef)
        {
            this.attractiveObjectInstance = MonoBehaviour.Instantiate(attractiveObjectInherentConfigurationData.AttractiveObjectModelPrefab);
            this.PlayerPocketObjectOutAnimationManager =
                new PlayerAnimationWithObjectManager(this.attractiveObjectInstance, playerManagerDataRetriever.GetPlayerAnimator(), attractiveObjectInherentConfigurationData.PreActionAnimation, 0f, false, null);
            this.PlayerObjectLayAnimationManager =
                 new PlayerAnimationWithObjectManager(this.attractiveObjectInstance, playerManagerDataRetriever.GetPlayerAnimator(), attractiveObjectInherentConfigurationData.PostActionAnimation, 0.05f, true, () => { attractiveObjectActionRef.OnObjectAnimationlayed(); });
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
            if (this.attractiveObjectInstance != null)
            {
                MonoBehaviour.Destroy(this.attractiveObjectInstance);
            }
        }

        public void OnAttractiveObjectActionOKInput()
        {
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

}
