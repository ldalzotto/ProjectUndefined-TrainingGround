using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class ObstaclesListenerManager : GameSingleton<ObstaclesListenerManager>
    {
        private int ObstacleListenerAddedCounter = 0;

        private List<ObstacleListenerObject> obstacleListeners = new List<ObstacleListenerObject>();

        public int OnObstacleListenerCreation(ObstacleListenerObject obstacleListener)
        {
            this.obstacleListeners.Add(obstacleListener);
            this.ObstacleListenerAddedCounter += 1;
            return this.ObstacleListenerAddedCounter;
        }

        public void OnObstacleListenerDestroyed(ObstacleListenerObject obstacleListener)
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
        public List<ObstacleListenerObject> GetAllObstacleListeners()
        {
            return this.obstacleListeners;
        }

        #endregion
        
    }

}
