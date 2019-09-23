using CoreGame;

namespace StartMenu
{
    public interface IGameProgressionStateManagerDataRetriever
    {
        bool HasAlreadyPlayed();
    }

    public class GameProgressionStateManager : IGameProgressionStateManagerDataRetriever
    {
        #region External Dependencies
        private GlobalGameConfiguration GlobalGameConfiguration;
        private StartLevelManager StartLevelManager;
        #endregion

        public void Init()
        {
            this.GlobalGameConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.GlobalGameConfiguration;
            this.StartLevelManager = CoreGameSingletonInstances.StartLevelManager;
        }

        #region IGameProgressionStateManagerDataRetriever
        public bool HasAlreadyPlayed()
        {
            return this.StartLevelManager.GetStartLevelID() != GameConfigurationID.LevelZonesID.NONE;
        }
        #endregion

    }
}
