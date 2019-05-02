using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class InRangeEffectManager : MonoBehaviour
    {
        #region External dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private CommandBuffer commandBuffer;

        private Dictionary<RangeTypeID, List<InRangeColliderTracker>> activeInRangeTrackers = new Dictionary<RangeTypeID, List<InRangeColliderTracker>>();

        public void Init()
        {
            this.commandBuffer = new CommandBuffer();
            this.commandBuffer.name = this.GetType().Name;
            Camera.main.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.commandBuffer);

            this.PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
        }

        #region External Events
        public void OnInRangeAdd(InRangeColliderTracker InRangeColliderTracker, RangeType rangeType)
        {
            if (rangeType.IsInRangeEffectEnabled())
            {
                if (!this.activeInRangeTrackers.ContainsKey(rangeType.RangeTypeID))
                {
                    this.activeInRangeTrackers[rangeType.RangeTypeID] = new List<InRangeColliderTracker>();
                }
                this.activeInRangeTrackers[rangeType.RangeTypeID].Add(InRangeColliderTracker);
            }
        }
        public void OnInRangeRemove(InRangeColliderTracker InRangeColliderTracker, RangeType rangeType)
        {
            if (rangeType.IsInRangeEffectEnabled())
            {
                var trackedRanges = this.activeInRangeTrackers[rangeType.RangeTypeID];
                bool delete = false;
                foreach (var trackedRange in trackedRanges)
                {
                    if (trackedRange == InRangeColliderTracker)
                    {
                        delete = true;
                        break;
                    }
                }
                if (delete)
                {
                    trackedRanges.Remove(InRangeColliderTracker);
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
                    foreach (var r in activeInRangeTracker.InvolvedRenderers)
                    {
                        this.commandBuffer.DrawRenderer(r, effectMaterial);
                    }
                }
            }
        }

    }
}
