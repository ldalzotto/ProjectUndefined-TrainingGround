using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class SquareObstaclesManager : MonoBehaviour
    {

        private List<SquareObstacle> squareObstacles;
        private List<SquareObstacle> lastFrameChangedObstacles;
        
        public void Init()
        {
            this.squareObstacles = GameObject.FindObjectsOfType<SquareObstacle>().ToList();
            this.lastFrameChangedObstacles = new List<SquareObstacle>();

            foreach (var squareObstacle in this.squareObstacles)
            {
                squareObstacle.Init();
            }
        }

        public void Tick(float d)
        {
            foreach (var squareObstacle in this.squareObstacles)
            {
                if (squareObstacle.Tick(d))
                {
                    this.lastFrameChangedObstacles.Add(squareObstacle);
                }
            }
        }

        public List<SquareObstacle> ConsumeLastFrameChangedObstacles()
        {
            var returnList = new List<SquareObstacle>(this.lastFrameChangedObstacles);
            this.lastFrameChangedObstacles.Clear();
            return returnList;
        }
    }
}
