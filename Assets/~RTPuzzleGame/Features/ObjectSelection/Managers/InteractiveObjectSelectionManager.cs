using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectSelectionManager : AbstractSelectableObjectSelectionManager<ISelectableModule>, IInteractiveObjectSelectionEvent
    {
        #region External Dependencies
        private PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager;
        #endregion

        public override SelectableObjectSelectionManagerEventListener<ISelectableModule> SelectableObjectSelectionManagerEventListener => this.PlayerActionPuzzleEventsManager;

        public override void Init(IGameInputManager GameInputManager)
        {
            this.PlayerActionPuzzleEventsManager = PuzzleGameSingletonInstances.PlayerActionPuzzleEventsManager;
            base.Init(GameInputManager);
        }

        #region IInteractiveObjectSelectionEvent
        public void OnSelectableEnter(ISelectableModule ISelectableModule)
        {
            this.AddInteractiveObjectFromSelectable(ISelectableModule);
        }

        public void OnSelectableExit(ISelectableModule ISelectableModule)
        {
            this.RemoveInteractiveObjectFromSelectable(ISelectableModule);
        }
        #endregion
        
    }
}
