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
            interactiveGameObject.CreateLogicCollider(ObstacleInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            this.ObstacleCollider = interactiveGameObject.GetLogicColliderAsBox();
            this.interactiveObjectTag = new InteractiveObjectTag { IsObstacle = true };
            this.squareObstacleSystem = new SquareObstacleSystem(this, ObstacleInteractiveObjectInitializerData.SquareObstacleSystemInitializationData);

            this.AfterConstructor();
        }

        public override void Destroy()
        {
            this.SquareObstacleSystem.Destroy();
            base.Destroy();
        }
    }
}
