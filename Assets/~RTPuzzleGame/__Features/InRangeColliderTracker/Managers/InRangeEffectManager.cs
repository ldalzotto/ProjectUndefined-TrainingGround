using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
using GameConfigurationID;

namespace RTPuzzle
{
    public class InRangeEffectManager : MonoBehaviour, IInRangeEffectManagerEvent
    {
        #region External dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private CommandBuffer commandBuffer;

        private Dictionary<RangeTypeID, List<IInRangeColliderTrackerModuleDataRetriever>> activeInRangeTrackers = new Dictionary<RangeTypeID, List<IInRangeColliderTrackerModuleDataRetriever>>();

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
                if (!this.activeInRangeTrackers.ContainsKey(triggeredRangeType.RangeTypeID))
                {
                    this.activeInRangeTrackers[triggeredRangeType.RangeTypeID] = new List<IInRangeColliderTrackerModuleDataRetriever>();
                }
                this.activeInRangeTrackers[triggeredRangeType.RangeTypeID].Add(InRangeColliderTrackerModule);
            }
        }
        public void OnInRangeRemove(IInRangeColliderTrackerModuleDataRetriever InRangeColliderTrackerModule, RangeType triggeredRangeType)
        {
            if (triggeredRangeType.IsInRangeEffectEnabled())
            {
                var trackedRanges = this.activeInRangeTrackers[triggeredRangeType.RangeTypeID];
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
                if (this.activeInRangeTrackers.ContainsKey(rangeType.RangeTypeID))
                {
                    this.activeInRangeTrackers[rangeType.RangeTypeID].Clear();
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
                var effectMaterial = this.PuzzleGameConfigurationManager.RangeTypeConfiguration()[listActiveInRangeTrackersPerId.Key].InRangeEffectMaterial;
                foreach (var activeInRangeTracker in listActiveInRangeTrackersPerId.Value)
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
