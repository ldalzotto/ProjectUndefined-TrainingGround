using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class SquareObstacleSystemManager 
    {
        private static SquareObstacleSystemManager Instance;
        public static SquareObstacleSystemManager Get()
        {
            if (Instance == null) { Instance = new SquareObstacleSystemManager(); }
            return Instance;
        }

        public int SquareObstacleSystemCounter { get; private set; } = 0;
        public int TotalFrustumCounter { get; private set; } = 0;

        public int OnSquareObstacleSystemCreated(SquareObstacleSystem SquareObstacleSystem)
        {
            this.SquareObstacleSystemCounter += 1;
            this.TotalFrustumCounter += SquareObstacleSystem.FaceFrustums.Count;
            return this.SquareObstacleSystemCounter;
        }

        public void OnSquareObstacleSystemDestroyed(SquareObstacleSystem SquareObstacleSystem)
        {
            this.TotalFrustumCounter -= SquareObstacleSystem.FaceFrustums.Count;
            this.SquareObstacleSystemCounter -= 1;
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }

}
