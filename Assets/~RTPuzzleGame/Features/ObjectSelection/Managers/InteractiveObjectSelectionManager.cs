using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectSelectionManager : AbstractSelectableObjectSelectionManager<ISelectableModule>, IInteractiveObjectSelectionEvent
    {
        #region External Dependencies
        private IPlayerActionManagerEvent IPlayerActionManagerEvent;
        #endregion

        public override SelectableObjectSelectionManagerEventListener<ISelectableModule> SelectableObjectSelectionManagerEventListener => this.IPlayerActionManagerEvent;

        public override void Init(IGameInputManager GameInputManager)
        {
            this.IPlayerActionManagerEvent = PuzzleGameSingletonInstances.PlayerActionManager;
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
