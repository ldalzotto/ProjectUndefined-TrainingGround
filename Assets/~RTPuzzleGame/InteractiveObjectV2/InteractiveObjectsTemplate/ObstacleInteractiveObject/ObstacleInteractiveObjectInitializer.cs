using UnityEngine;
using System.Collections;
using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    public class ObstacleInteractiveObjectInitializerData
    {
        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData;

        [DrawNested]
        public InteractiveObjectLogicCollider InteractiveObjectLogicCollider;
    }

    [System.Serializable]
    [SceneHandleDraw]
    public class ObstacleInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested]
        public ObstacleInteractiveObjectInitializerData InteractiveObjectInitializerData;

        public override void Init()
        {
            new ObstacleInteractiveObject(new InteractiveGameObject(this.gameObject), InteractiveObjectInitializerData);
        }
    }
}