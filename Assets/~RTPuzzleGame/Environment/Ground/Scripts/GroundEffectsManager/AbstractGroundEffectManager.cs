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
        List<MeshRenderer> MeshRenderToRender(List<GroundEffectType> affectedGroundEffectsType);
        List<GroundEffectType> GroundEffectTypeToRender(List<GroundEffectType> affectedGroundEffectsType);
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

        public List<MeshRenderer> MeshRenderToRender(List<GroundEffectType> affectedGroundEffectsType)
        {
            List<MeshRenderer> involvedRenderers = new List<MeshRenderer>();
            foreach (var affectedGroundEffectType in affectedGroundEffectsType)
            {
                if (affectedGroundEffectType.MeshRenderer.isVisible
                    && this.associatedRangeObject.RangeType.GetCollider().bounds.Intersects(affectedGroundEffectType.MeshRenderer.bounds)) //render only intersected geometry
                {
                    involvedRenderers.Add(affectedGroundEffectType.MeshRenderer);
                }
            }
            return involvedRenderers;
        }

        public List<GroundEffectType> GroundEffectTypeToRender(List<GroundEffectType> affectedGroundEffectsType)
        {
            List<GroundEffectType> involvedGroundEffectType = new List<GroundEffectType>();
            foreach (var affectedGroundEffectType in affectedGroundEffectsType)
            {
                if (affectedGroundEffectType.MeshRenderer.isVisible
                    && this.associatedRangeObject.RangeType.GetCollider().bounds.Intersects(affectedGroundEffectType.MeshRenderer.bounds)) //render only intersected geometry
                {
                    involvedGroundEffectType.Add(affectedGroundEffectType);
                }
            }
            return involvedGroundEffectType;
        }




    }

}
