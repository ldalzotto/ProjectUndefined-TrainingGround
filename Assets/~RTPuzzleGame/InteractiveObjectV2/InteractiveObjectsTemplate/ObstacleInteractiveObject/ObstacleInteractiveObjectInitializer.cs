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
    public class ObstacleInteractiveObjectInitializer : A_InteractiveObjectInitializer
    {
        public ObstacleInteractiveObjectInitializerData InteractiveObjectInitializerData;

        protected override object GetInitializerDataObject()
        {
            return this.InteractiveObjectInitializerData;
        }

        protected override CoreInteractiveObject GetInteractiveObject()
        {
            return new ObstacleInteractiveObject(new InteractiveGameObject(this.gameObject), InteractiveObjectInitializerData);
        }
    }
}