using System.Collections.Generic;

namespace RTPuzzle
{
    public interface IGameSingleton
    {
        void OnDestroy();
    }
    public abstract class GameSingleton<T> : IGameSingleton where T : IGameSingleton, new() 
    {
        private static T Instance;
        public static T Get()
        {
            if (Instance == null)
            {
                Instance = new T();
                GameSingletonManagers.Get().OnGameSingletonCreated(Instance);
            }
            return Instance;
        }

        public virtual void OnDestroy()
        {
            Instance = default;
        }
    }

    public class GameSingletonManagers
    {
        private static GameSingletonManagers Instance;
        public static GameSingletonManagers Get()
        {
            if (Instance == null) { Instance = new GameSingletonManagers(); }
            return Instance;
        }

        private List<IGameSingleton> AllGameSingletons = new List<IGameSingleton>();

        public void OnGameSingletonCreated(IGameSingleton GameSingleton)
        {
            this.AllGameSingletons.Add(GameSingleton);
        }

        public void OnDestroy()
        {
            foreach(var gameSingleton in this.AllGameSingletons)
            {
                gameSingleton.OnDestroy();
            }
            Instance = null;
        }
    }
}
