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
                attractiveObject = MonoBehaviour.Instantiate(attractiveObjectInherentConfigurationData.AttractiveObjectPrefab, parent);
            }
            else
            {
                attractiveObject = MonoBehaviour.Instantiate(attractiveObjectInherentConfigurationData.AttractiveObjectPrefab);
            }

            attractiveObject.transform.position = worldPosition;
            attractiveObject.Init(attractiveObjectInherentConfigurationData);
            return attractiveObject;
        }

        public static AttractiveObjectType GetAttractiveObjectFromCollisionType(CollisionType collisionType)
        {
            var sphereRange = RangeType.RetrieveFromCollisionType(collisionType);
            if (sphereRange != null)
            {
                return sphereRange.GetComponentInParent<AttractiveObjectType>();
            }
            return null;
        }

        #region Internal Dependencies
        private SphereRangeType sphereRange;
        #endregion

        public AttractiveObjectId AttractiveObjectId;
        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;

        public void Init(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData)
        {
            #region External Dependencies
            var attractiveObjectContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
            #endregion
            this.sphereRange = GetComponentInChildren<SphereRangeType>();
            this.sphereRange.Init(attractiveObjectInherentConfigurationData.EffectRange);

            this.attractiveObjectInherentConfigurationData = attractiveObjectInherentConfigurationData;
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(attractiveObjectInherentConfigurationData.EffectiveTime);

            attractiveObjectContainerManager.OnAttracteObjectCreated(this);
        }

        private AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData;

        public AttractiveObjectInherentConfigurationData AttractiveObjectInherentConfigurationData { get => attractiveObjectInherentConfigurationData; }

        public bool Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
            return this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }

        #region 
        public bool IsInRangeOf(Vector3 compareWorldPosition)
        {
            return this.sphereRange.IsInside(compareWorldPosition);
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
