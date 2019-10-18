using InteractiveObjects;
using UnityEngine;

namespace Obstacle
{
    public class ObstacleInteractiveObject : CoreInteractiveObject
    {
        [VE_Nested] private SquareObstacleOcclusionFrustumsDefinition squareObstacleOcclusionFrustumsDefinition;

        public ObstacleInteractiveObject(IInteractiveGameObject interactiveGameObject, ObstacleInteractiveObjectInitializerData ObstacleInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            interactiveGameObject.CreateLogicCollider(ObstacleInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            this.ObstacleCollider = interactiveGameObject.GetLogicColliderAsBox();
            this.SquareObstacleSystemInitializationData = ObstacleInteractiveObjectInitializerData.SquareObstacleSystemInitializationData;
            this.interactiveObjectTag = new InteractiveObjectTag {IsObstacle = true};
            this.squareObstacleOcclusionFrustumsDefinition = new SquareObstacleOcclusionFrustumsDefinition();
            this.AfterConstructor();

            this.ObstacleInteractiveObjectUniqueID = ObstacleInteractiveObjectManager.Get().OnSquareObstacleSystemCreated(this);
        }

        public int ObstacleInteractiveObjectUniqueID { get; private set; }
        public BoxCollider ObstacleCollider { get; private set; }
        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData { get; private set; }
        public SquareObstacleOcclusionFrustumsDefinition SquareObstacleOcclusionFrustumsDefinition => squareObstacleOcclusionFrustumsDefinition;

        #region Data Retrieval

        /// <summary>
        /// The center of the square obstacle is used by the occlusion frustums calculator.
        /// </summary>
        public TransformStruct GetObstacleCenterTransform()
        {
            return this.InteractiveGameObject.GetLogicColliderCenterTransform();
        }

        #endregion

        public override void Destroy()
        {
            ObstacleInteractiveObjectManager.Get().OnSquareObstacleSystemDestroyed(this);
            base.Destroy();
        }
    }
}