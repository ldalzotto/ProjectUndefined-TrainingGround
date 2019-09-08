using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectSelectionManager : AbstractSelectableObjectSelectionManager<ISelectableModule>
    {
        #region External Dependencies
        private PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager;
        #endregion

        public override SelectableObjectSelectionManagerEventListener<ISelectableModule> SelectableObjectSelectionManagerEventListener => this.PlayerActionPuzzleEventsManager;

        public override void Init(IGameInputManager GameInputManager)
        {
            this.PlayerActionPuzzleEventsManager = GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>();
            base.Init(GameInputManager);
        }

        #region External Events
        public void OnActionInteractableEnter(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.AddInteractiveObjectFromSelectable(actionInteractableObjectModule);
        }

        public void OnActionInteractableExit(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.RemoveInteractiveObjectFromSelectable(actionInteractableObjectModule);
        }

        public void OnGrabObjectEnter(GrabObjectModule grabObjectModule)
        {
            this.AddInteractiveObjectFromSelectable(grabObjectModule);
        }

        public void OnGrabObjectExit(GrabObjectModule grabObjectModule)
        {
            this.RemoveInteractiveObjectFromSelectable(grabObjectModule);
        }
        #endregion
        
    }
}
