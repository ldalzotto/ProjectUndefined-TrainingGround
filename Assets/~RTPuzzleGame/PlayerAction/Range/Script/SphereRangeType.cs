using UnityEngine;
using System.Collections;
using System;

namespace RTPuzzle
{
    public class SphereRangeType : RangeType
    {
        #region Internal components
        private SphereCollider sphereCollider;
        #endregion

        private Func<Vector3> originPositionProvider;

        public void Init(float sphereRadius, Func<Vector3> originPositionProvider = null, Func<Color> rangeColorProvider = null)
        {
            this.originPositionProvider = originPositionProvider;
            this.sphereCollider = GetComponent<SphereCollider>();
            this.sphereCollider.radius = sphereRadius;
            base.Init();
            if (this.IsRangeConfigurationDefined())
            {
                this.rangeTypeInherentConfigurationData.RangeColorProvider = rangeColorProvider;
            }
        }

        public static SphereRangeType Instanciate(RangeTypeID rangeTypeID, float sphereRadius, Func<Vector3> originPositionProvider = null, Func<Color> rangeColorProvider = null)
        {
            var rangeTypeContainer = GameObject.FindObjectOfType<RangeTypeContainer>();
            var sphereRangeType = MonoBehaviour.Instantiate(PrefabContainer.Instance.BaseSphereRangePrefab, rangeTypeContainer.transform);
            sphereRangeType.RangeTypeID = rangeTypeID;
            sphereRangeType.Init(sphereRadius, originPositionProvider, rangeColorProvider);
            return sphereRangeType;
        }

        public override void Tick(float d)
        {
            if (this.originPositionProvider != null)
            {
                transform.position = this.originPositionProvider.Invoke();
            }
        }

        public override bool IsInside(Vector3 worldPointComparison)
        {
            return Vector3.Distance(this.GetCenterWorldPos(), worldPointComparison) <= this.sphereCollider.radius;
        }

        public override float GetRadiusRange()
        {
            return this.sphereCollider.radius;
        }
        public Vector3 GetCenterWorldPos()
        {
            return transform.TransformPoint(this.sphereCollider.center);
        }

        public override Collider GetCollider()
        {
            return this.sphereCollider;
        }
    }
}
