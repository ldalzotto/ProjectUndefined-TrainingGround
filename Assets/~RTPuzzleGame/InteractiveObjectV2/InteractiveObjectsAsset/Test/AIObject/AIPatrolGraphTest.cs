using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using SequencedAction;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "AIPatrolGraphTest", menuName = "Test/AIPatrolGraphTest", order = 1)]
    public class AIPatrolGraphTest : AIPatrolGraphV2
    {
        [GraphPatrolPointAttribute(R = 0f, G = 1f, B = 0f)] [GraphPatrolLine(fieldTargetWorldPosition: nameof(AIPatrolGraphTest.P3))]
        public AIMoveToActionInputData P1;

        [GraphPatrolPointAttribute] [GraphPatrolLine(fieldTargetWorldPosition: nameof(AIPatrolGraphTest.P1))]
        public AIMoveToActionInputData P2;

        [GraphPatrolPointAttribute] [GraphPatrolLine(fieldTargetWorldPosition: nameof(AIPatrolGraphTest.P2))]
        public AIMoveToActionInputData P3;

        public override List<ASequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject)
        {
            return new List<ASequencedAction>()
            {
                new AIWarpActionV2(InvolvedInteractiveObject, this.TransformToWorldPosition(this.P1.WorldPoint), () => new List<ASequencedAction>()
                {
                    new BranchInfiniteLoopAction(
                        new List<ASequencedAction>()
                        {
                            this.CreateAIMoveToActionV2(InvolvedInteractiveObject, this.P2, () => new List<ASequencedAction>()
                            {
                                this.CreateAIMoveToActionV2(InvolvedInteractiveObject, this.P1, () => new List<ASequencedAction>()
                                {
                                    this.CreateAIMoveToActionV2(InvolvedInteractiveObject, this.P3, null)
                                })
                            })
                        }
                    )
                })
            };
        }
    }
}