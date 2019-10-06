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

        private int ObstacleListenerCounter = 0;
        public int TotalNearObstaclesCounter { get; private set; } = 0;

        private List<ObstacleListener> obstacleListeners = new List<ObstacleListener>();

        public int OnObstacleListenerCreation(ObstacleListener obstacleListener)
        {
            this.obstacleListeners.Add(obstacleListener);
            this.ObstacleListenerCounter += 1;
            return this.ObstacleListenerCounter;
        }

        public void OnObstacleListenerDestroyed(ObstacleListener obstacleListener)
        {
            this.obstacleListeners.Remove(obstacleListener);
            this.ObstacleListenerCounter -= 1;
            this.TotalNearObstaclesCounter -= obstacleListener.NearSquareObstacles.Count;
        }

        public void OnAddedNearObstacleToObstacleListener(ObstacleListener obstacleListener, ObstacleInteractiveObject obstacleInteractiveObject)
        {
            this.TotalNearObstaclesCounter += 1;
        }

        public void OnRemovedNearObstacleToObstacleListener(ObstacleListener obstacleListener, ObstacleInteractiveObject obstacleInteractiveObject)
        {
            this.TotalNearObstaclesCounter -= 1;
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
