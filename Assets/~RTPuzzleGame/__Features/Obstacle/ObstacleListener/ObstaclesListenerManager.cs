using InteractiveObjectTest;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class ObstaclesListenerManager
    {
        private static ObstaclesListenerManager Instance;
        public static ObstaclesListenerManager Get()
        {
            if (Instance == null) { Instance = new ObstaclesListenerManager(); }
            return Instance;
        }

        private int ObstacleListenerAddedCounter = 0;

        private List<ObstacleListener> obstacleListeners = new List<ObstacleListener>();

        public int OnObstacleListenerCreation(ObstacleListener obstacleListener)
        {
            this.obstacleListeners.Add(obstacleListener);
            this.ObstacleListenerAddedCounter += 1;
            return this.ObstacleListenerAddedCounter;
        }

        public void OnObstacleListenerDestroyed(ObstacleListener obstacleListener)
        {
            this.obstacleListeners.Remove(obstacleListener);
        }

        #region Data Retrieval
        public List<ObstacleListener> GetAllObstacleListeners()
        {
            return this.obstacleListeners;
        }

        #endregion

        public void OnDestroy()
        {
            Instance = null;
        }
    }

}
