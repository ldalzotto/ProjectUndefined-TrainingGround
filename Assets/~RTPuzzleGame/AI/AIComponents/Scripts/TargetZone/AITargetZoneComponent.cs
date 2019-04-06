﻿using System;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : AbstractAIComponent
    {
        public float TargetZoneEscapeDistance;

        protected override Type abstractManagerType => typeof(AbstractAITargetZoneManager);
    }

    public abstract class AbstractAITargetZoneManager
    {
        #region State
        protected bool isEscapingFromTargetZone;
        #endregion
   
        public bool IsEscapingFromTargetZone { get => isEscapingFromTargetZone; }

        public abstract Nullable<Vector3> TickComponent();
        public abstract void TriggerTargetZoneEscape(TargetZone targetZone);
        public abstract void OnDestinationReached();
        public abstract void OnStateReset();
    }

}
