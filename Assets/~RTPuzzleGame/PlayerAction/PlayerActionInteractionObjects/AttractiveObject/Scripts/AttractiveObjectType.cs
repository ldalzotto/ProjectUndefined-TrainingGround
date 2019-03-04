using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectType : MonoBehaviour
    {

        public static AttractiveObjectType Instanciate(Vector3 worldPosition, Transform parent, AttractiveObjectId attractiveObjectId)
        {
            var attractiveObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.AttractiveObjectPrefab, parent);
            attractiveObject.transform.position = worldPosition;
            attractiveObject.Init(attractiveObjectId);
            return attractiveObject;
        }

        public static AttractiveObjectType GetAttractiveObjectFromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponent<AttractiveObjectType>();
        }

        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;

        public void Init(AttractiveObjectId attractiveObjectId)
        {
            this.attractiveObjectId = attractiveObjectId;
            var sphereCollider = GetComponent<SphereCollider>();
            var attractiveObjectInherentConfigurationData = AttractiveObjectConfiguration.conf[attractiveObjectId];
            sphereCollider.radius = attractiveObjectInherentConfigurationData.EffectRange;
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(attractiveObjectInherentConfigurationData.EffectiveTime);
        }

        private AttractiveObjectId attractiveObjectId;

        public bool Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
            return this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }

        #region 
        public bool IsInRangeOf(Vector3 compareWorldPosition)
        {
            return Vector3.Distance(transform.position, compareWorldPosition) < AttractiveObjectConfiguration.conf[attractiveObjectId].EffectRange;
        }
        #endregion

    }

    class AttractiveObjectLifetimeTimer
    {
        private float effectiveTime;

        public AttractiveObjectLifetimeTimer(float effectiveTime)
        {
            this.effectiveTime = effectiveTime;
        }

        private float elapsedTime;

        #region Logical Condition
        public bool IsTimeOver()
        {
            return elapsedTime >= effectiveTime;
        }
        #endregion

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.elapsedTime += (d * timeAttenuationFactor);
        }

    }
}
