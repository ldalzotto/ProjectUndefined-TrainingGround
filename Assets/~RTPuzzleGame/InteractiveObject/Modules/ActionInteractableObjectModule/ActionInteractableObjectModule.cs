using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class ActionInteractableObjectModule : InteractiveObjectModule
    {
        [CustomEnum()]
        public ActionInteractableObjectID ActionInteractableObjectID;

        #region Module Dependencies
        #endregion

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private RTPPlayerAction associatedPlayerAction;
        private ActionInteractableObjectInherentData ActionInteractableObjectInherentData;

        #region Data Retrieval
        public RTPPlayerAction AssociatedPlayerAction { get => associatedPlayerAction; }
        #endregion

        public void Init(ActionInteractableObjectInherentData ActionInteractableObjectInherentData, PuzzleGameConfigurationManager puzzleGameConfigurationManager, PuzzleEventsManager PuzzleEventsManager)
        {
            this.ActionInteractableObjectInherentData = ActionInteractableObjectInherentData;
            this.PuzzleEventsManager = PuzzleEventsManager;
            var triggerCollider = GetComponent<SphereCollider>();
            triggerCollider.radius = this.ActionInteractableObjectInherentData.InteractionRange;

            this.associatedPlayerAction = new CutsceneAction((CutsceneActionInherentData)puzzleGameConfigurationManager.PlayerActionConfiguration()[ActionInteractableObjectInherentData.PlayerActionId]);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.PuzzleEventsManager.PZ_EVT_OnActionInteractableEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.PuzzleEventsManager.PZ_EVT_OnActionInteractableExit(this);
            }
        }

        public static class ActionInteractableObjectModuleInstancer
        {
            public static void PopuplateFromDefinition(
                              ActionInteractableObjectModule ActionInteractableObjectModule,
                              ActionInteractableObjectModuleDefinition ActionInteractableObjectModuleDefinition)
            {
                ActionInteractableObjectModule.ActionInteractableObjectID = ActionInteractableObjectModuleDefinition.ActionInteractableObjectID;
            }
        }

    }
}

