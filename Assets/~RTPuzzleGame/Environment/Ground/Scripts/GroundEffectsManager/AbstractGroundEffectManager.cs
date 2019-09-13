using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;

namespace RTPuzzle
{
    public interface IAbstractGroundEffectManager
    {
        void OnRangeCreated(RangeTypeObject rangeTypeObject);
        void Tick(float d, List<GroundEffectType> affectedGroundEffectsType);
        RangeTypeObject GetAssociatedRangeObject();
        void OnRangeDestroyed();
        bool MeshMustBeRebuild();
        List<GroundEffectType> GroundEffectTypeToRender();
    }

    public abstract class AbstractGroundEffectManager<T> : IAbstractGroundEffectManager
    {
        private RangeTypeObject associatedRangeObject;
        private List<GroundEffectType> groundEffectTypesToRender;
        protected RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;
        protected FloatAnimation rangeAnimation;
        protected bool isAttractiveObjectRangeEnabled;
        private bool isGroundEffectTypeToRenderChanged;

        public RangeTypeObject GetAssociatedRangeObject()
        {
            return associatedRangeObject;
        }

        public AbstractGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData)
        {
            this.rangeTypeInherentConfigurationData = rangeTypeInherentConfigurationData;
            this.isGroundEffectTypeToRenderChanged = false;
            this.groundEffectTypesToRender = new List<GroundEffectType>();
        }

        public void Tick(float d, List<GroundEffectType> affectedGroundEffectsType)
        {
            if (isAttractiveObjectRangeEnabled)
            {
                this.rangeAnimation.Tick(d);
            }

            List<GroundEffectType> involvedGroundEffectsType = new List<GroundEffectType>();

            if (affectedGroundEffectsType != null)
            {
                foreach (var affectedGroundEffectType in affectedGroundEffectsType)
                {
                    if (affectedGroundEffectType.MeshRenderer.isVisible
                        && this.associatedRangeObject.RangeType.GetCollider().bounds.Intersects(affectedGroundEffectType.MeshRenderer.bounds)) //render only intersected geometry
                    {
                        involvedGroundEffectsType.Add(affectedGroundEffectType);
                    }
                }

                this.isGroundEffectTypeToRenderChanged = false;
                if (involvedGroundEffectsType.Count != this.groundEffectTypesToRender.Count)
                {
                    this.isGroundEffectTypeToRenderChanged = true;
                }
                if (!this.isGroundEffectTypeToRenderChanged)
                {
                    foreach (var involvedGroundEffectType in involvedGroundEffectsType)
                    {
                        if (!this.groundEffectTypesToRender.Contains(involvedGroundEffectType))
                        {
                            this.isGroundEffectTypeToRenderChanged = true;
                            break;
                        }
                    }
                }

            }
            else
            {
                this.isGroundEffectTypeToRenderChanged = true;
            }

            this.groundEffectTypesToRender = involvedGroundEffectsType;

        }

        public void OnRangeCreated(RangeTypeObject rangeTypeObject)
        {
            this.associatedRangeObject = rangeTypeObject;
            this.rangeAnimation = new FloatAnimation(this.associatedRangeObject.RangeType.GetRadiusRange(), rangeTypeInherentConfigurationData.RangeAnimationSpeed, 0f);
            this.isAttractiveObjectRangeEnabled = true;
            this.Tick(0, null);
        }

        public void OnRangeDestroyed()
        {
            this.isAttractiveObjectRangeEnabled = false;
        }

        public RangeTypeID GetRangeTypeID()
        {
            return this.associatedRangeObject.RangeType.RangeTypeID;
        }

        public bool MeshMustBeRebuild()
        {
            return this.isGroundEffectTypeToRenderChanged;
        }

        public List<GroundEffectType> GroundEffectTypeToRender()
        {
            return this.groundEffectTypesToRender;
        }
    }

}
