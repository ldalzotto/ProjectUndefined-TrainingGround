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
    public class ObstacleInteractiveObjectInitializer : AInteractiveObjectInitializer
    {
        public ObstacleInteractiveObjectInitializerData InteractiveObjectInitializerData;
        public override void Init()
        {
            InteractiveObjectV2Manager.Get().OnInteractiveObjectCreated(new ObstacleInteractiveObject(new InteractiveGameObject(this.gameObject), InteractiveObjectInitializerData));
        }
    }
}