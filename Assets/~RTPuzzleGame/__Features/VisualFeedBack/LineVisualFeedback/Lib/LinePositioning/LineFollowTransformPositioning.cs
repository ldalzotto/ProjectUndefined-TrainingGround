﻿using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class LineFollowTransformPositioning : ILinePositioning
    {
        private Transform TransformToFollow;
        private Vector3 targetWorldPositionOffset;

        public LineFollowTransformPositioning(Transform TransformToFollow, ExtendedBounds AverageModelBounds)
        {
            this.TransformToFollow = TransformToFollow;
            this.targetWorldPositionOffset = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(AverageModelBounds);
        }
        
        public Vector3 GetEndPosition(Vector3 startPosition)
        {
            return this.TransformToFollow.position + this.targetWorldPositionOffset;
        }
    }

}
