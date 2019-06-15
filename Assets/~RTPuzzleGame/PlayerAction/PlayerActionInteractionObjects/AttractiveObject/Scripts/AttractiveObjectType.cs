using UnityEngine;
using CoreGame;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    public class AttractiveObjectType : MonoBehaviour, IRenderBoundRetrievable
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

        #region Properties
        private ExtendedBounds AverageModeBounds;
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

            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(attractiveObjectInherentConfigurationData.EffectiveTime);

            this.AverageModeBounds = BoundsHelper.GetAverageRendererBounds(this.GetComponentsInChildren<Renderer>());

            attractiveObjectContainerManager.OnAttracteObjectCreated(this);
        }

        public bool Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
            return this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }

        #region Logical Conditions
        public bool IsInRangeOf(Vector3 compareWorldPosition)
        {
            return this.sphereRange.IsInside(compareWorldPosition);
        }
        #endregion

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.yellow;
            Handles.Label(transform.position + new Vector3(0, 3f, 0), AttractiveObjectId.ToString(), labelStyle);
            Gizmos.DrawIcon(transform.position + new Vector3(0, 5.5f, 0), "Gizmo_AttractiveObject", true);
#endif
        }

        #region Data Retrieval
        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return this.AverageModeBounds;
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
