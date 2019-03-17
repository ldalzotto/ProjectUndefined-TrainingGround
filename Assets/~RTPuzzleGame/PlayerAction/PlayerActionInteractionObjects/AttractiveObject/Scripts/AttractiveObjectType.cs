using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectType : MonoBehaviour
    {
        
        public static AttractiveObjectType Instanciate(Vector3 worldPosition, Transform parent, AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData)
        {
            AttractiveObjectType attractiveObject = null;
            if (parent != null)
            {
                attractiveObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.AttractiveObjectPrefab, parent);
            }
            else
            {
                attractiveObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.AttractiveObjectPrefab);
            }

            attractiveObject.transform.position = worldPosition;
            attractiveObject.Init(attractiveObjectInherentConfigurationData);
            return attractiveObject;
        }

        public static AttractiveObjectType GetAttractiveObjectFromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponent<AttractiveObjectType>();
        }

        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;

        public void Init(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData)
        {
            #region External Dependencies
            var attractiveObjectContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
            #endregion
            
            var sphereCollider = GetComponent<SphereCollider>();
            this.attractiveObjectInherentConfigurationData = attractiveObjectInherentConfigurationData;
            sphereCollider.radius = attractiveObjectInherentConfigurationData.EffectRange;
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(attractiveObjectInherentConfigurationData.EffectiveTime);

            attractiveObjectContainerManager.OnAttracteObjectCreated(this);
        }

        private AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData;

        public bool Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
            return this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }

        #region 
        public bool IsInRangeOf(Vector3 compareWorldPosition)
        {
            return Vector3.Distance(transform.position, compareWorldPosition) < this.attractiveObjectInherentConfigurationData.EffectRange;
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
