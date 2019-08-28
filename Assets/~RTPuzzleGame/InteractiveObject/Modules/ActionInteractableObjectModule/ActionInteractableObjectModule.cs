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

        private InteractiveObjectType baseInteractiveObjectType;

        private RTPPlayerAction associatedPlayerAction;
        private ActionInteractableObjectInherentData ActionInteractableObjectInherentData;

        #region Data Retrieval
        public RTPPlayerAction AssociatedPlayerAction { get => associatedPlayerAction; }
        #endregion

        public void Init(ActionInteractableObjectModuleInitializationData ActionInteractableObjectModuleInitializationData, InteractiveObjectType baseInteractiveObjectType, PuzzleGameConfigurationManager puzzleGameConfigurationManager, PuzzleEventsManager PuzzleEventsManager)
        {
            this.ActionInteractableObjectInherentData = ActionInteractableObjectModuleInitializationData.ActionInteractableObjectInherentData;
            this.baseInteractiveObjectType = baseInteractiveObjectType;
            this.PuzzleEventsManager = PuzzleEventsManager;
            var triggerCollider = GetComponent<SphereCollider>();
            triggerCollider.radius = this.ActionInteractableObjectInherentData.InteractionRange;

            this.associatedPlayerAction = new CutsceneAction((CutsceneActionInherentData)ActionInteractableObjectModuleInitializationData.AssociatedPlayerActionInherentData);
        }

        public void TickAlways(float d)
        {
            if (!this.associatedPlayerAction.HasStillSomeExecutionAmount())
            {
                this.baseInteractiveObjectType.DisableModule(this.GetType());
            }
        }

        public override void OnModuleDisabled()
        {
            this.PuzzleEventsManager.PZ_EVT_OnActionInteractableExit(this);
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

public class ActionInteractableObjectModuleInitializationData
    {
        public ActionInteractableObjectInherentData ActionInteractableObjectInherentData;
        public PlayerActionInherentData AssociatedPlayerActionInherentData;
    }
}

