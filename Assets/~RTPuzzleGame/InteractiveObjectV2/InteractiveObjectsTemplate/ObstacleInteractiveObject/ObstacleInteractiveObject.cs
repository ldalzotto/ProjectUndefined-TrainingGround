using RTPuzzle;
using UnityEngine;

namespace InteractiveObjectTest
{
    public class ObstacleInteractiveObject : CoreInteractiveObject
    {
        public BoxCollider ObstacleCollider { get; private set; }

        public SquareObstacleSystem SquareObstacleSystem { get; private set; }

        public ObstacleInteractiveObject(InteractiveGameObject interactiveGameObject, ObstacleInteractiveObjectInitializerData ObstacleInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.ObstacleCollider = interactiveGameObject.LogicCollider;
            this.InteractiveObjectTag = new InteractiveObjectTag { IsObstacle = true };
            this.SquareObstacleSystem = new SquareObstacleSystem(this, ObstacleInteractiveObjectInitializerData.SquareObstacleSystemInitializationData);
        }

        public override void Destroy()
        {
            this.SquareObstacleSystem.Destroy();
            base.Destroy();
        }
    }
}
