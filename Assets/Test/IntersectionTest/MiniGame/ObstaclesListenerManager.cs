using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstaclesListenerManager : MonoBehaviour
{
    private List<ObstacleListener> obstacleListeners;

    public void Init()
    {
        this.obstacleListeners = new List<ObstacleListener>();
        foreach (var obstacleListener in GameObject.FindObjectsOfType<ObstacleListener>().ToList())
        {
            obstacleListener.Init();
        }
    }

    public void OnObstacleListenerCreation(ObstacleListener obstacleListener)
    {
        this.obstacleListeners.Add(obstacleListener);
    }

    #region Data Retrieval
    public List<ObstacleListener> GetAllObstacleListeners()
    {
        return this.obstacleListeners;
    }
    #endregion

    public void Tick(float d)
    {
        foreach (var obstacleListener in this.obstacleListeners)
        {
            obstacleListener.Tick(d);
        }
    }
}
