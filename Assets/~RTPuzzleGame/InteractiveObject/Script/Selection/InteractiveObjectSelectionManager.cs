using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectSelectionManager : AbstractSelectableObjectSelectionManager<SelectableObject>
    {
        #region External Dependencies
        private PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager;
        #endregion

        public override SelectableObjectSelectionManagerEventListener<SelectableObject> SelectableObjectSelectionManagerEventListener => this.PlayerActionPuzzleEventsManager;

        public override void Init(IGameInputManager GameInputManager)
        {
            this.PlayerActionPuzzleEventsManager = GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>();
            base.Init(GameInputManager);
        }

        #region External Events
        public void OnActionInteractableEnter(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.AddInteractiveObjectFromSelectable(actionInteractableObjectModule, new SelectableObject(actionInteractableObjectModule.GetModelObjectModule(), actionInteractableObjectModule.AssociatedPlayerAction));
        }

        public void OnActionInteractableExit(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.RemoveInteractiveObjectFromSelectable(actionInteractableObjectModule);
        }

        public void OnGrabObjectEnter(GrabObjectModule grabObjectModule)
        {
            this.AddInteractiveObjectFromSelectable(grabObjectModule, new SelectableObject(grabObjectModule.ModelObjectModule, grabObjectModule.GrabObjectAction));
        }

        public void OnGrabObjectExit(GrabObjectModule grabObjectModule)
        {
            this.RemoveInteractiveObjectFromSelectable(grabObjectModule);
        }
        #endregion


    }

    public class SelectableObject : AbstractSelectableObject
    {
        public RTPPlayerAction AssociatedPlayerAction;

        public SelectableObject(IRenderBoundRetrievable modelObjectModule, RTPPlayerAction AssociatedPlayerAction) : base(modelObjectModule)
        {
            this.AssociatedPlayerAction = AssociatedPlayerAction;
        }
    }
}
