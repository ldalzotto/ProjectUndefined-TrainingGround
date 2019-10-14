
using CoreGame;
using InteractiveObjectTest;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public class SquareObstacleSystemInitializationData
    {
        [Tooltip("Avoid tracking of value every frame. But obstacle frustum will never be updated")]
        public bool IsStatic = true;
    }

    public class SquareObstacleSystem
    {
        public int squareObstacleSystemUniqueID;
        public int SquareObstacleSystemUniqueID { get => squareObstacleSystemUniqueID; }

        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData { get; private set; }
        public List<FrustumV2> FaceFrustums { get; private set; }

        #region Internal Dependencies
        [VE_Ignore]
        private ObstacleInteractiveObject AssociatedInteractiveObject;
        #endregion

        #region Data Retrieval
        public TransformStruct GetTransform()
        {
            return new TransformStruct
            {
                WorldPosition = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition +
                          this.AssociatedInteractiveObject.ObstacleCollider.center,
                WorldRotation = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldRotation,
                LossyScale = (this.AssociatedInteractiveObject.ObstacleCollider != null ? this.AssociatedInteractiveObject.ObstacleCollider.size : Vector3.one).Mul(this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().LossyScale)
            };
        }
        #endregion

        public SquareObstacleSystem(ObstacleInteractiveObject AssociatedInteractiveObject, SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.SquareObstacleSystemInitializationData = SquareObstacleSystemInitializationData;

            this.FaceFrustums = new List<FrustumV2>();

            //Create frustum for all sides -> occlusions are only calculated for facing frustums.
            this.CreateAndAddFrustum(Quaternion.Euler(0, 0, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 180, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, -90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(90, 0, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(-90, 0, 0), 1);

            this.squareObstacleSystemUniqueID = SquareObstacleSystemManager.Get().OnSquareObstacleSystemCreated(this);
        }

        public void Destroy()
        {
            SquareObstacleSystemManager.Get().OnSquareObstacleSystemDestroyed(this);
        }

        private void CreateAndAddFrustum(Quaternion deltaRotation, float F1FaceZOffset)
        {
            var frustum = new FrustumV2();
            frustum.FaceDistance = 9999f;
            frustum.F1.Width = 1f;
            frustum.F1.Height = 1f;
            frustum.DeltaRotation = deltaRotation;
            frustum.F1.FaceOffsetFromCenter.z = F1FaceZOffset;
            this.FaceFrustums.Add(frustum);
        }
    }

}
