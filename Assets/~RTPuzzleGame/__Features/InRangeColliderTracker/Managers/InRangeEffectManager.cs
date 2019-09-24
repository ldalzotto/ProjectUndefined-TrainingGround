using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class InRangeEffectManager : MonoBehaviour, IInRangeEffectManagerEvent
    {
        #region External dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private CommandBuffer commandBuffer;

        private Dictionary<RangeType, List<IInRangeColliderTrackerModuleDataRetriever>> activeInRangeTrackers = new Dictionary<RangeType, List<IInRangeColliderTrackerModuleDataRetriever>>();

        public void Init()
        {
            this.commandBuffer = new CommandBuffer();
            this.commandBuffer.name = this.GetType().Name;
            Camera.main.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.commandBuffer);

            this.PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
        }

        #region External Events
        public void OnInRangeAdd(IInRangeColliderTrackerModuleDataRetriever InRangeColliderTrackerModule, RangeType triggeredRangeType)
        {
            if (triggeredRangeType.IsInRangeEffectEnabled())
            {
                if (!this.activeInRangeTrackers.ContainsKey(triggeredRangeType))
                {
                    this.activeInRangeTrackers[triggeredRangeType] = new List<IInRangeColliderTrackerModuleDataRetriever>();
                }
                this.activeInRangeTrackers[triggeredRangeType].Add(InRangeColliderTrackerModule);
            }
        }
        public void OnInRangeRemove(IInRangeColliderTrackerModuleDataRetriever InRangeColliderTrackerModule, RangeType triggeredRangeType)
        {
            if (triggeredRangeType.IsInRangeEffectEnabled())
            {
                var trackedRanges = this.activeInRangeTrackers[triggeredRangeType];
                bool delete = false;
                foreach (var trackedRange in trackedRanges)
                {
                    if (trackedRange == InRangeColliderTrackerModule)
                    {
                        delete = true;
                        break;
                    }
                }
                if (delete)
                {
                    trackedRanges.Remove(InRangeColliderTrackerModule);
                }
            }
        }

        public void OnRangeDestroy(RangeType rangeType)
        {
            if (rangeType.IsInRangeEffectEnabled())
            {
                if (this.activeInRangeTrackers.ContainsKey(rangeType))
                {
                    this.activeInRangeTrackers[rangeType].Clear();
                }
            }
        }
        #endregion

        public void Tick(float d)
        {
            this.OnCommandBufferUpdate();
        }

        private void OnCommandBufferUpdate()
        {
            this.commandBuffer.Clear();

            foreach (var listActiveInRangeTrackersPerId in this.activeInRangeTrackers)
            {
                var effectMaterial = this.PuzzleGameConfigurationManager.RangeTypeConfiguration()[listActiveInRangeTrackersPerId.Key.RangeTypeID].InRangeEffectMaterial;
                foreach (var activeInRangeTracker in listActiveInRangeTrackersPerId.Value)
                {
                    var activeInRangeTrackerAIColliderModule = activeInRangeTracker.GetAILogicColliderModule();
                    if ((activeInRangeTrackerAIColliderModule != null && listActiveInRangeTrackersPerId.Key.IsInsideAndNotOccluded(activeInRangeTrackerAIColliderModule.GetCollider(), forceObstacleOcclusionIfNecessary: false))
                       || (activeInRangeTrackerAIColliderModule == null)
                   )
                    {
                        foreach (var r in activeInRangeTracker.ModelObjectModule.GetAllRenderers())
                        {
                            this.commandBuffer.DrawRenderer(r, effectMaterial);
                        }
                    }
                }
            }
        }
    }
}