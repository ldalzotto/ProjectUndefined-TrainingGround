using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class SquareObstaclesManager : MonoBehaviour
    {

        private List<SquareObstacleSystem> squareObstacleSystems = new List<SquareObstacleSystem>();
        private List<SquareObstacleSystem> lastFrameChangedObstacles = new List<SquareObstacleSystem>();

        public void Tick(float d)
        {
            foreach (var squareObstacleSystem in this.squareObstacleSystems)
            {
                if (squareObstacleSystem.Tick(d))
                {
                    this.lastFrameChangedObstacles.Add(squareObstacleSystem);
                }
            }
        }

        public void AddSquareObstacleSystem(SquareObstacleSystem SquareObstacleSystem)
        {
            this.squareObstacleSystems.Add(SquareObstacleSystem);
        }

        public List<SquareObstacleSystem> ConsumeLastFrameChangedObstacles()
        {
            var returnList = new List<SquareObstacleSystem>(this.lastFrameChangedObstacles);
            this.lastFrameChangedObstacles.Clear();
            return returnList;
        }
    }
}
