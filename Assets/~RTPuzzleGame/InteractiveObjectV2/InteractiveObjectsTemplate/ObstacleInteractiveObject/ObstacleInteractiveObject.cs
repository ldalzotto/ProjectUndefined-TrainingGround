using RTPuzzle;
using UnityEngine;

namespace InteractiveObjectTest
{
    public class ObstacleInteractiveObject : CoreInteractiveObject
    {
        public BoxCollider ObstacleCollider { get; private set; }

        [VE_Nested]
        public SquareObstacleSystem squareObstacleSystem;
        public SquareObstacleSystem SquareObstacleSystem { get => squareObstacleSystem; }

        public ObstacleInteractiveObject(InteractiveGameObject interactiveGameObject, ObstacleInteractiveObjectInitializerData ObstacleInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.ObstacleCollider = interactiveGameObject.GetLogicColliderAsBox();
            this.interactiveObjectTag = new InteractiveObjectTag { IsObstacle = true };
            this.squareObstacleSystem = new SquareObstacleSystem(this, ObstacleInteractiveObjectInitializerData.SquareObstacleSystemInitializationData);
        }

        public override void Destroy()
        {
            this.SquareObstacleSystem.Destroy();
            base.Destroy();
        }
    }
}
