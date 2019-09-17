using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class ActionInteractableObjectModule : InteractiveObjectModule, ISelectableModule
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
        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            if (this.modelObjectModule != null)
            {
                return this.modelObjectModule.GetAverageModelBoundLocalSpace();
            }
            return default(ExtendedBounds);
        }
        public RTPPlayerAction GetAssociatedPlayerAction()
        {
            return this.associatedPlayerAction;
        }
        #endregion

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            ActionInteractableObjectInherentData ActionInteractableObjectInherentData = interactiveObjectInitializationObject.ActionInteractableObjectInherentData;
            if (ActionInteractableObjectInherentData == null)
            {
                ActionInteractableObjectInherentData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.ActionInteractableObjectConfiguration()[this.ActionInteractableObjectID];
            }

            this.ActionInteractableObjectInherentData = ActionInteractableObjectInherentData;
            this.baseInteractiveObjectType = interactiveObjectType;
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            this.modelObjectModule = interactiveObjectType.GetModule<ModelObjectModule>();
            var triggerCollider = GetComponent<SphereCollider>();
            triggerCollider.radius = this.ActionInteractableObjectInherentData.InteractionRange;

            this.associatedPlayerAction = new CutsceneAction((CutsceneActionInherentData)PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.PlayerActionConfiguration()[ActionInteractableObjectInherentData.PlayerActionId]);
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

