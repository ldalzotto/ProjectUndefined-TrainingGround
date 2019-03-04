using UnityEngine;
using System.Collections;
using System;

namespace RTPuzzle
{
    public class AttractiveObjectAction : RTPPlayerAction
    {
        public override SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId => SelectionWheelNodeConfigurationId.ATTRACTIVE_OBJECT_LAY_WHEEL_CONFIG;

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;
        #endregion

        #region Internal Managers
        private AttractiveObjectInputManager AttractiveObjectInputManager;
        private AttractiveObjectGroundPositioner AttractiveObjectGroundPositioner;
        #endregion

        private bool isActionOver;
        private AttractiveObjectId AttractiveObjectId;

        public AttractiveObjectAction(AttractiveObjectId AttractiveObjectId, float coolDownTime) : base(coolDownTime)
        {
            this.AttractiveObjectId = AttractiveObjectId;
        }

        public override bool FinishedCondition()
        {
            return isActionOver;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            this.isActionOver = false;

            #region External Dependencies
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.AttractiveObjectsContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
            var playerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            #endregion

            this.AttractiveObjectInputManager = new AttractiveObjectInputManager(gameInputManager);
            this.AttractiveObjectGroundPositioner = new AttractiveObjectGroundPositioner(playerDataRetriever.GetPlayerTransform());

            this.PuzzleEventsManager.OnAttractiveObjectActionStart(AttractiveObjectConfiguration.conf[AttractiveObjectId], playerDataRetriever.GetPlayerTransform());

        }

        public override void GizmoTick()
        {
            
        }

        public override void GUITick()
        {
        
        }

        public override void Tick(float d)
        {
            this.AttractiveObjectInputManager.Tick();
            if (this.AttractiveObjectInputManager.ActionButtonPressed)
            {
                this.EnAction();
                var objectSpawnPosition = this.AttractiveObjectGroundPositioner.GetAttractiveObjectSpawnPosition();
                if (objectSpawnPosition.HasValue)
                {
                    this.AttractiveObjectsContainerManager.OnAttractiveObjectActionExecuted(objectSpawnPosition.Value, AttractiveObjectId);
                }
               
                ResetCoolDown();
            }
            if (this.AttractiveObjectInputManager.ReturnButtonPressed)
            {
                this.EnAction();
            }
        }

        private void EnAction()
        {
            this.PuzzleEventsManager.OnAttractiveObjectActionEnd();
            this.isActionOver = true;
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

        public bool ActionButtonPressed { get => actionButtonPressed;  }
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

        public Nullable<Vector3> GetAttractiveObjectSpawnPosition()
        {
            int groundLayerMask = 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER);
            RaycastHit hit;
            if(Physics.Raycast(playerTransform.position, Vector3.down, out hit, Mathf.Infinity, groundLayerMask))
            {
                return hit.point;
            }else
            {
                return null;
            }
        }
        
    }

}
