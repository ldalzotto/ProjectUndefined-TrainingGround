using CoreGame;

namespace RTPuzzle
{
    public class InteractiveObjectSelectionManager : AbstractSelectableObjectSelectionManager<ISelectableModule>, IGameSingleton
    {
        private static InteractiveObjectSelectionManager Instance;
        public static InteractiveObjectSelectionManager Get()
        {
            if (Instance == null)
            {
                GameSingletonManagers.Get().OnGameSingletonCreated(Instance);
                Instance = new InteractiveObjectSelectionManager();
            }
            return Instance;
        }
        
        public override SelectableObjectSelectionManagerEventListener<ISelectableModule> SelectableObjectSelectionManagerEventListener => PlayerActionManager.Get();

        public override void Init(IGameInputManager GameInputManager)
        {
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

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}
