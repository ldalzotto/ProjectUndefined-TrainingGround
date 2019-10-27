namespace CoreGame
{
    public abstract class ConfigurationGameObjectManager<T> : GameSingleton<T> where T : IGameSingleton, new()
    {
        protected ConfigurationGameObjectManager()
        {
        }
    }
}