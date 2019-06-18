using UnityEngine;
using System.Collections;
using CoreGame;
using System.Collections.Generic;
using GameConfigurationID;

namespace RTPuzzle
{
    public interface IAbstractGroundEffectManager
    {
        void OnRangeCreated(RangeType rangeType);
        void Tick(float d);
        void MeshToRender(ref HashSet<MeshRenderer> renderers, GroundEffectType[] affectedGroundEffectsType);
        void OnRangeDestroyed();
    }

    public abstract class AbstractGroundEffectManager<T> : IAbstractGroundEffectManager where T : RangeType
    {
        protected T associatedRange;
        protected RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;
        protected FloatAnimation rangeAnimation;
        protected bool isAttractiveObjectRangeEnabled;

        public bool IsAttractiveObjectRangeEnabled { get => isAttractiveObjectRangeEnabled; }

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

        public void OnRangeCreated(RangeType rangeType)
        {
            this.associatedRange = (T)rangeType;
            this.rangeAnimation = new FloatAnimation(rangeType.GetRadiusRange(), rangeTypeInherentConfigurationData.RangeAnimationSpeed, 0f);
            this.isAttractiveObjectRangeEnabled = true;
            this.Tick(0);
        }

        public void OnRangeDestroyed()
        {
            this.isAttractiveObjectRangeEnabled = false;
        }

        public RangeTypeID GetRangeTypeID()
        {
            return this.associatedRange.RangeTypeID;
        }

        public void MeshToRender(ref HashSet<MeshRenderer> renderers, GroundEffectType[] affectedGroundEffectsType)
        {
            foreach (var affectedGroundEffectType in affectedGroundEffectsType)
            {
                if (affectedGroundEffectType.MeshRenderer.isVisible
                    && this.associatedRange.GetCollider().bounds.Intersects(affectedGroundEffectType.MeshRenderer.bounds)) //render only intersected geometry
                {
                    renderers.Add(affectedGroundEffectType.MeshRenderer);
                }
            }
        }
        
    }

}
