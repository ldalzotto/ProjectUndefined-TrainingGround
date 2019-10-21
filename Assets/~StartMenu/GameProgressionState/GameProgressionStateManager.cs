using CoreGame;
using LevelManagement;

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
        private StartLevelManager StartLevelManager = StartLevelManager.Get();

        #endregion

        public void Init()
        {
            this.GlobalGameConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.GlobalGameConfiguration;
        }

        #region IGameProgressionStateManagerDataRetriever

        public bool HasAlreadyPlayed()
        {
            return this.StartLevelManager.GetStartLevelID() != LevelZonesID.NONE;
        }

        #endregion
    }
}