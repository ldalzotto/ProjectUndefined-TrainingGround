using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        public List<SquareObstacleSystem> AllSquareObstacleSystems { get; private set; } = new List<SquareObstacleSystem>();
        public int SquareObstacleSystemAddedCounter { get; private set; } = 0;

        public int OnSquareObstacleSystemCreated(SquareObstacleSystem SquareObstacleSystem)
        {
            this.AllSquareObstacleSystems.Add(SquareObstacleSystem);
            this.SquareObstacleSystemAddedCounter += 1;
            return this.SquareObstacleSystemAddedCounter;
        }

        public void OnSquareObstacleSystemDestroyed(SquareObstacleSystem SquareObstacleSystem)
        {
            this.AllSquareObstacleSystems.Remove(SquareObstacleSystem);
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }

}
