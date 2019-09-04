using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationSystem
{
    public class AnimationActorMovementModule : AbstractAnimationActorModule
    {
        #region Module dependencies
        private AnimationActorModelModule AnimationActorModelModule;
        #endregion

        private AnimationActor AnimationActorRef;
        private AnimationActorModelModuleDefinition AnimationActorModelModuleDefinition;

        private AnimationActorMovementManager AnimationActorMovementManager;

        public override void Init(AnimationActor AnimationActorRef)
        {
            this.AnimationActorRef = AnimationActorRef;
            this.AnimationActorModelModuleDefinition = AnimationActorRef.AnimationActorDefinition.AnimationActorModelModuleDefinition;
            this.AnimationActorModelModule = AnimationActorRef.GetModule<AnimationActorModelModule>();

            this.AnimationActorMovementManager = new AnimationActorMovementManager(this.AnimationActorModelModuleDefinition, this.AnimationActorModelModule, AnimationActorRef);

            var initialPosition = this.AnimationActorModelModuleDefinition.destinationPositions[Random.Range(0, this.AnimationActorModelModuleDefinition.destinationPositions.Count)];
            this.AnimationActorRef.transform.position = initialPosition.position;
            this.AnimationActorRef.transform.rotation = initialPosition.rotation;
        }

        public override void SeenByCameraTick(float d)
        {
            this.AnimationActorMovementManager.SeenByCameraTick(d);
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.OnPlayerTriggerStayOrEnter(collisionType);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.OnPlayerTriggerStayOrEnter(collisionType);
            }
        }

        private Vector3 lastTriggerEventPlayerPosition;

        private void OnPlayerTriggerStayOrEnter(CollisionType CollisionType)
        {
            if (lastTriggerEventPlayerPosition != CollisionType.transform.position)
            {
                this.AnimationActorMovementManager.Escape(CollisionType.transform.position);
            }
        }

        private void OnTriggerExit(Collider other)
        {

        }
    }

    class AnimationActorMovementManager
    {
        #region Module dependencies
        private AnimationActorModelModule AnimationActorModelModule;
        private AnimationActor AnimationActorRef;
        #endregion

        private AnimationActorModelModuleDefinition AnimationActorModelModuleDefinition;

        public AnimationActorMovementManager(AnimationActorModelModuleDefinition AnimationActorModelModuleDefinition, AnimationActorModelModule AnimationActorModelModule, AnimationActor AnimationActorRef)
        {
            this.AnimationActorModelModuleDefinition = AnimationActorModelModuleDefinition;
            this.AnimationActorModelModule = AnimationActorModelModule;
            this.AnimationActorRef = AnimationActorRef;
            this.isMoving = false;
            this.targetPosition = null;
        }



        private bool isMoving;
        private Transform targetPosition;

        public void SeenByCameraTick(float d)
        {
            if (this.isMoving)
            {
                this.AnimationActorRef.transform.position = Vector3.MoveTowards(this.AnimationActorRef.transform.position, this.targetPosition.position, d * this.AnimationActorModelModuleDefinition.speed);
                this.isMoving = (this.AnimationActorRef.transform.position != this.targetPosition.position);
                if (!this.isMoving)
                {
                    this.AnimationActorRef.transform.position = this.targetPosition.transform.position;
                    this.AnimationActorRef.transform.rotation = this.targetPosition.transform.rotation;
                    this.AnimationActorModelModule.PlayAnimation(AnimationID.POSE_OVERRIVE_LISTENING, this.AnimationActorModelModuleDefinition.moveAnimationCrossFade);
                }
            }
        }

        public void Escape(Vector3 escapePoint)
        {
            if (!this.isMoving)
            {
                var targetPosition = this.AnimationActorModelModuleDefinition.destinationPositions[Random.Range(0, this.AnimationActorModelModuleDefinition.destinationPositions.Count)];
                if (this.targetPosition == null || targetPosition.position != this.targetPosition.position)
                {
                    this.SetTargetPosition(targetPosition);
                }
            }
        }

        private void SetTargetPosition(Transform targetPosition)
        {
            this.targetPosition = targetPosition;
            if (!this.isMoving)
            {
                this.AnimationActorModelModule.PlayAnimation(this.AnimationActorModelModuleDefinition.moveAnimationID, this.AnimationActorModelModuleDefinition.moveAnimationCrossFade);
                this.AnimationActorRef.transform.rotation = Quaternion.LookRotation((this.targetPosition.transform.position - this.AnimationActorRef.transform.position).normalized, this.AnimationActorRef.transform.up);
                this.isMoving = true;
            }

        }


    }

    [System.Serializable]
    public class AnimationActorModelModuleDefinition
    {
        public bool isEnabled;
        public float triggerRange;
        public float speed;
        [CustomEnum()]
        public AnimationID moveAnimationID;
        public float moveAnimationCrossFade;
        public List<Transform> destinationPositions;
    }

}
