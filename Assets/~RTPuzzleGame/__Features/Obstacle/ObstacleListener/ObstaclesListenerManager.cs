using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstaclesListenerManager : MonoBehaviour
    {
        private List<ObstacleListener> obstacleListeners;

        public void Init()
        {
            this.obstacleListeners = new List<ObstacleListener>();
        }

        public void OnObstacleListenerCreation(ObstacleListener obstacleListener)
        {
            this.obstacleListeners.Add(obstacleListener);
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
    }

}
