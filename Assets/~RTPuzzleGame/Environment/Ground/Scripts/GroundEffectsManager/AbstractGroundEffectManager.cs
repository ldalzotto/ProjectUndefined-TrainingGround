using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface IAbstractGroundEffectManager
    {
        void OnRangeCreated(RangeTypeObject rangeTypeObject);
        void Tick(float d);
        void MeshToRender(ref HashSet<MeshRenderer> renderers, GroundEffectType[] affectedGroundEffectsType);
        RangeTypeObject GetAssociatedRangeObject();
        void OnRangeDestroyed();
    }

    public abstract class AbstractGroundEffectManager<T> : IAbstractGroundEffectManager
    {
        private RangeTypeObject associatedRangeObject;
        protected RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;
        protected FloatAnimation rangeAnimation;
        protected bool isAttractiveObjectRangeEnabled;

        public bool IsAttractiveObjectRangeEnabled { get => isAttractiveObjectRangeEnabled; }
        public RangeTypeObject GetAssociatedRangeObject()
        {
            return associatedRangeObject;
        }

        public AbstractGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData)
        {
            this.rangeTypeInherentConfigurationData = rangeTypeInherentConfigurationData;
            this.isAttractiveObjectRangeEnabled = false;
        }

        public void Tick(float d)
        {
            if (isAttractiveObjectRangeEnabled)
            {
                this.rangeAnimation.Tick(d);
            }
        }

        public void OnRangeCreated(RangeTypeObject rangeTypeObject)
        {
            this.associatedRangeObject = rangeTypeObject;
            this.rangeAnimation = new FloatAnimation(this.associatedRangeObject.RangeType.GetRadiusRange(), rangeTypeInherentConfigurationData.RangeAnimationSpeed, 0f);
            this.isAttractiveObjectRangeEnabled = true;
            this.Tick(0);
        }

        public void OnRangeDestroyed()
        {
            this.isAttractiveObjectRangeEnabled = false;
        }

        public RangeTypeID GetRangeTypeID()
        {
            return this.associatedRangeObject.RangeType.RangeTypeID;
        }

        public void MeshToRender(ref HashSet<MeshRenderer> renderers, GroundEffectType[] affectedGroundEffectsType)
        {
            foreach (var affectedGroundEffectType in affectedGroundEffectsType)
            {
                if (affectedGroundEffectType.MeshRenderer.isVisible
                    && this.associatedRangeObject.RangeType.GetCollider().bounds.Intersects(affectedGroundEffectType.MeshRenderer.bounds)) //render only intersected geometry
                {
                    renderers.Add(affectedGroundEffectType.MeshRenderer);
                }
            }
        }

    }

}
