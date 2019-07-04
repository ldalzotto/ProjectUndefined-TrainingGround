using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquareObstaclesManager : MonoBehaviour
{

    private List<SquareObstacle> squareObstacles;

    public void Init()
    {
        this.squareObstacles = GameObject.FindObjectsOfType<SquareObstacle>().ToList();
        foreach(var squareObstacle in this.squareObstacles)
        {
            squareObstacle.Init();
        }
    }
}