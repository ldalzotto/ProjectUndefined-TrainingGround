using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;

namespace RTPuzzle
{
    public interface IAbstractGroundEffectManager
    {
        void OnRangeCreated(ARangeObjectRenderingDataProvider rangeObjectRenderingDataProvider);
        void Tick(float d, List<GroundEffectType> affectedGroundEffectsType);
        bool MeshMustBeRebuild();
        List<GroundEffectType> GroundEffectTypeToRender();
        ObstacleListenerObject GetObstacleListener();
    }

    public abstract class AbstractGroundEffectManager : IAbstractGroundEffectManager
    {
        protected ARangeObjectRenderingDataProvider rangeObjectRenderingDataProvider;
        private List<GroundEffectType> groundEffectTypesToRender;
        protected RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;
        private bool isGroundEffectTypeToRenderChanged;
        
        public AbstractGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData)
        {
            this.rangeTypeInherentConfigurationData = rangeTypeInherentConfigurationData;
            this.isGroundEffectTypeToRenderChanged = false;
            this.groundEffectTypesToRender = new List<GroundEffectType>();
        }

        public void Tick(float d, List<GroundEffectType> affectedGroundEffectsType)
        {
            List<GroundEffectType> involvedGroundEffectsType = new List<GroundEffectType>();

            if (affectedGroundEffectsType != null)
            {
                foreach (var affectedGroundEffectType in affectedGroundEffectsType)
                {
                    if (affectedGroundEffectType.MeshRenderer.isVisible
                        && this.rangeObjectRenderingDataProvider.BoundingCollider.bounds.Intersects(affectedGroundEffectType.MeshRenderer.bounds)) //render only intersected geometry
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

        public void OnRangeCreated(ARangeObjectRenderingDataProvider rangeObjectRenderingDataProvider)
        {
            this.rangeObjectRenderingDataProvider = rangeObjectRenderingDataProvider;
            this.Tick(0, null);
        }

        public RangeTypeID GetRangeTypeID()
        {
            return this.rangeObjectRenderingDataProvider.RangeTypeID;
        }

        public bool MeshMustBeRebuild()
        {
            return this.isGroundEffectTypeToRenderChanged;
        }

        public List<GroundEffectType> GroundEffectTypeToRender()
        {
            return this.groundEffectTypesToRender;
        }

        public ObstacleListenerObject GetObstacleListener()
        {
            return this.rangeObjectRenderingDataProvider.ObstacleListener;
        }
    }

}
