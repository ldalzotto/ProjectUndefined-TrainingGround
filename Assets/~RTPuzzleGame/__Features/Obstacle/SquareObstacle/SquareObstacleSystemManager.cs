using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CoreGame;

namespace RTPuzzle
{
    public class SquareObstacleSystemManager : GameSingleton<SquareObstacleSystemManager>
    {
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
    }

}
