using GameConfigurationID;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class SphereRangeType : RangeType
    {
        #region Internal components
        private SphereCollider sphereCollider;
        #endregion

        private Func<Vector3> originPositionProvider;

        public override void PopulateFromDefinition(RangeTypeDefinition rangeTypeDefinition)
        {
            base.PopulateFromDefinition(rangeTypeDefinition);
            if(rangeTypeDefinition.RangeShapeConfiguration != null && rangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(SphereRangeShapeConfiguration))
            {
                SphereRangeShapeConfiguration SphereRangeShapeConfiguration = (SphereRangeShapeConfiguration)rangeTypeDefinition.RangeShapeConfiguration;
                this.PopupulateFromData(SphereRangeShapeConfiguration.Radius);
            }
        }

        public void PopupulateFromData(float radius)
        {
            this.sphereCollider = GetComponent<SphereCollider>();
            this.sphereCollider.radius = radius;
        }

        public override void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, RangeTypeObject RangeTypeObjectRef)
        {
            this.sphereCollider = GetComponent<SphereCollider>();

            if (RangeTypeObjectInitializer != null)
            {
                this.originPositionProvider = RangeTypeObjectInitializer.OriginPositionProvider;
            }

            base.Init(RangeTypeObjectInitializer, RangeTypeObjectRef);

            if (RangeTypeObjectInitializer != null)
            {
                if (this.IsRangeConfigurationDefined())
                {
                    this.rangeTypeInherentConfigurationData.RangeColorProvider = RangeTypeObjectInitializer.RangeColorProvider;
                }
            }
        }

      

        public override void Tick(float d)
        {
            if (this.originPositionProvider != null)
            {
                transform.position = this.originPositionProvider.Invoke();
            }
        }

        public override bool IsInside(BoxCollider boxCollider)
        {
            return false;
        }

        public override bool IsInside(Vector3 worldPointComparison)
        {
            return Vector3.Distance(this.GetCenterWorldPos(), worldPointComparison) <= this.sphereCollider.radius;
        }

        public override float GetRadiusRange()
        {
            return this.sphereCollider.radius;
        }
        public override Collider GetCollider()
        {
            return this.sphereCollider;
        }

        public override Vector3 GetCenterWorldPos()
        {
            return transform.TransformPoint(this.sphereCollider.center);
        }
    }
}
