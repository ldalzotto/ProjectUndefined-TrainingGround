using UnityEngine;
using System.Collections;
using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class ObstacleInteractiveObjectInitializerData
    {
        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData;
    }

    [System.Serializable]
    public class ObstacleInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        public ObstacleInteractiveObjectInitializerData InteractiveObjectInitializerData;

        public override void Init()
        {
            new ObstacleInteractiveObject(new InteractiveGameObject(this.gameObject), InteractiveObjectInitializerData);
        }
    }
}