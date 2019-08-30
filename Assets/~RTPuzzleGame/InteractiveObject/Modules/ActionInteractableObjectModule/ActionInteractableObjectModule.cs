using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class ActionInteractableObjectModule : InteractiveObjectModule
    {
        [CustomEnum()]
        public ActionInteractableObjectID ActionInteractableObjectID;

        #region Module Dependencies
        private ModelObjectModule modelObjectModule;
        #endregion

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private InteractiveObjectType baseInteractiveObjectType;

        private RTPPlayerAction associatedPlayerAction;
        private ActionInteractableObjectInherentData ActionInteractableObjectInherentData;

        #region Data Retrieval
        public RTPPlayerAction AssociatedPlayerAction { get => associatedPlayerAction; }
        public ModelObjectModule GetModelObjectModule()
        {
            return this.modelObjectModule;
        }
        #endregion

        public void Init(ActionInteractableObjectInherentData ActionInteractableObjectInherentData, InteractiveObjectType baseInteractiveObjectType, 
            PuzzleGameConfigurationManager puzzleGameConfigurationManager, PuzzleEventsManager PuzzleEventsManager, ModelObjectModule ModelObjectModule)
        {
            this.ActionInteractableObjectInherentData = ActionInteractableObjectInherentData;
            this.baseInteractiveObjectType = baseInteractiveObjectType;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.modelObjectModule = ModelObjectModule;
            var triggerCollider = GetComponent<SphereCollider>();
            triggerCollider.radius = this.ActionInteractableObjectInherentData.InteractionRange;

            this.associatedPlayerAction = new CutsceneAction((CutsceneActionInherentData)puzzleGameConfigurationManager.PlayerActionConfiguration()[ActionInteractableObjectInherentData.PlayerActionId]);
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

}

