using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIAttractiveObjectComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIAttractiveObjectComponent", order = 1)]
    public class AIAttractiveObjectComponent : AbstractAIComponent
    {
        protected override Type abstractManagerType => typeof(AbstractAIAttractiveObjectManager);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AIAttractiveObjectComponent))]
    public class AIAttractiveObjectComponentEditor : AbstractAIComponentEditor<AIAttractiveObjectComponent>
    { }
#endif

    public abstract class AbstractAIAttractiveObjectManager
    {

        protected NavMeshAgent selfAgent;
        protected AiID aiID;

        #region State
        protected bool isAttracted;
        #endregion
        protected AttractiveObjectType involvedAttractiveObject;

        public abstract Vector3? TickComponent();

        #region External Events
        public abstract void OnTriggerEnter(Collider collider, CollisionType collisionType);
        public abstract void OnTriggerStay(Collider collider, CollisionType collisionType);
        public abstract void OnTriggerExit(Collider collider, CollisionType collisionType);
        public abstract void OnDestinationReached();

        public void ClearAttractedObject()
        {
            this.involvedAttractiveObject = null;
        }
        #endregion

        #region Logical Conditions
        public bool IsDestructedAttractiveObjectEqualsToCurrent(AttractiveObjectType attractiveObjectToDestroy)
        {
            return (this.involvedAttractiveObject != null &&
                attractiveObjectToDestroy.GetInstanceID() == this.involvedAttractiveObject.GetInstanceID());
        }
        public bool IsInfluencedByAttractiveObject()
        {
            return this.isAttracted || this.HasSensedThePresenceOfAnAttractiveObject();
        }
        public bool HasSensedThePresenceOfAnAttractiveObject()
        {
            return (this.involvedAttractiveObject != null && this.involvedAttractiveObject.IsInRangeOf(this.selfAgent.transform.position));
        }
        #endregion
    }

}
