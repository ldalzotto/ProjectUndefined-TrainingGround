using CoreGame;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectAction : RTPPlayerAction
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;
        #endregion

        #region Internal Managers
        private AttractiveObjectInputManager AttractiveObjectInputManager;
        private AttractiveObjectGroundPositioner AttractiveObjectGroundPositioner;
        private AttractiveObjectPlayerAnimationManager AttractiveObjectPlayerAnimationManager;
        #endregion

        private bool isActionOver;
        private AttractiveObjectId attractiveObjectId;

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
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.AttractiveObjectsContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
            var playerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            #endregion

            this.AttractiveObjectInputManager = new AttractiveObjectInputManager(gameInputManager);
            this.AttractiveObjectGroundPositioner = new AttractiveObjectGroundPositioner(playerDataRetriever.GetPlayerTransform());
            this.AttractiveObjectPlayerAnimationManager = new AttractiveObjectPlayerAnimationManager(playerDataRetriever, this);
            this.PuzzleEventsManager.OnAttractiveObjectActionStart(puzzleConfigurationManager.AttractiveObjectsConfiguration()[this.attractiveObjectId], playerDataRetriever.GetPlayerTransform());
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
            this.PuzzleEventsManager.OnAttractiveObjectActionEnd();
            this.isActionOver = true;
        }
        #endregion
    }

    class AttractiveObjectPlayerAnimationManager
    {
        private PlayerAnimationWithObjectManager PlayerPocketObjectOutAnimationManager;
        private PlayerAnimationWithObjectManager PlayerObjectLayAnimationManager;
        private GameObject attractiveObjectInstance;

        public AttractiveObjectPlayerAnimationManager(PlayerManagerDataRetriever playerManagerDataRetriever, AttractiveObjectAction attractiveObjectActionRef)
        {
            this.attractiveObjectInstance = MonoBehaviour.Instantiate(PrefabContainer.Instance.AttractiveObjectModelPrefab);
            this.PlayerPocketObjectOutAnimationManager =
                new PlayerAnimationWithObjectManager(this.attractiveObjectInstance, playerManagerDataRetriever.GetPlayerAnimator(), PlayerAnimatioNamesEnum.PLAYER_ACTION_CA_POCKET_ITEM, 0f, false, null);
            this.PlayerObjectLayAnimationManager =
                 new PlayerAnimationWithObjectManager(this.attractiveObjectInstance, playerManagerDataRetriever.GetPlayerAnimator(), PlayerAnimatioNamesEnum.PLAYER_ACTION_CA_POCKET_ITEM_LAY, 0.05f, true, () => { attractiveObjectActionRef.OnObjectAnimationlayed(); });
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
            if (this.attractiveObjectInstance != null) {
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
        private Transform playerTransform;

        public AttractiveObjectGroundPositioner(Transform playerTransform)
        {
            this.playerTransform = playerTransform;
        }

        public Nullable<RaycastHit> GetAttractiveObjectSpawnPosition()
        {
            int groundLayerMask = 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER);
            RaycastHit hit;
            if (Physics.Raycast(playerTransform.position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
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
