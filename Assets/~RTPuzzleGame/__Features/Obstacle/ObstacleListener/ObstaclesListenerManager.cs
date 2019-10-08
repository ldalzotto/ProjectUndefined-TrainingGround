using CoreGame;
using System.Collections.Generic;
using UnityEngine;

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

        #region Debug Display
        public void GizmoTick()
        {
            ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
            foreach (var obstacleListener in this.obstacleListeners)
            {
                ObstacleOcclusionCalculationManagerV2.TryGetCalculatedOcclusionFrustumsForObstacleListener(obstacleListener, out Dictionary<int, List<FrustumPointsPositions>> allCalculatedFrustumPositions);
                if (allCalculatedFrustumPositions != null)
                {
                    foreach (var calculatedFrustumPositions in allCalculatedFrustumPositions.Values)
                    {
                        foreach (var calculatedFrustumPosition in calculatedFrustumPositions)
                        {
                            calculatedFrustumPosition.DrawInScene(MyColors.GetColorOnIndex(obstacleListener.ObstacleListenerUniqueID));
                        }
                    }
                }
            }
        }
        #endregion

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
