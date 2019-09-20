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
        private IActionInteractableObjectModuleEventListener IActionInteractableObjectModuleEventListener;
        #endregion

        private IInteractiveObjectTypeEvents associatedIInteractiveObjectTypeEvents;

        private RTPPlayerAction associatedPlayerAction;
        private ActionInteractableObjectInherentData ActionInteractableObjectInherentData;

        #region ISelectableModule
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

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
                IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            ActionInteractableObjectInherentData ActionInteractableObjectInherentData = interactiveObjectInitializationObject.ActionInteractableObjectInherentData;
            if (ActionInteractableObjectInherentData == null)
            {
                ActionInteractableObjectInherentData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.ActionInteractableObjectConfiguration()[this.ActionInteractableObjectID];
            }

            this.ActionInteractableObjectInherentData = ActionInteractableObjectInherentData;
            this.associatedIInteractiveObjectTypeEvents = IInteractiveObjectTypeEvents;
            this.IActionInteractableObjectModuleEventListener = PuzzleGameSingletonInstances.PuzzleEventsManager;
            this.modelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();
            var triggerCollider = GetComponent<SphereCollider>();
            triggerCollider.radius = this.ActionInteractableObjectInherentData.InteractionRange;

            this.associatedPlayerAction = new CutsceneAction((CutsceneActionInherentData)PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.PlayerActionConfiguration()[ActionInteractableObjectInherentData.PlayerActionId]);
        }

        public void TickAlways(float d)
        {
            if (!this.associatedPlayerAction.HasStillSomeExecutionAmount())
            {
                this.associatedIInteractiveObjectTypeEvents.DisableModule(this.GetType());
            }
        }

        public override void OnModuleDisabled()
        {
            this.IActionInteractableObjectModuleEventListener.PZ_EVT_OnActionInteractableExit(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.IActionInteractableObjectModuleEventListener.PZ_EVT_OnActionInteractableEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.IActionInteractableObjectModuleEventListener.PZ_EVT_OnActionInteractableExit(this);
            }
        }
    }

}

