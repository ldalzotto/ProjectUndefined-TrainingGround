using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class LevelCompletionConditionResolutionInput
    {
        private NPCAIManagerContainer nPCAIManagerContainer;
        private TargetZoneContainer targetZoneContainer;

        public LevelCompletionConditionResolutionInput(NPCAIManagerContainer nPCAIManagerContainer, TargetZoneContainer targetZoneContainer)
        {
            this.nPCAIManagerContainer = nPCAIManagerContainer;
            this.targetZoneContainer = targetZoneContainer;
        }

        public NPCAIManagerContainer NPCAIManagerContainer { get => nPCAIManagerContainer; }
        public TargetZoneContainer TargetZoneContainer { get => targetZoneContainer; }
    }
}
