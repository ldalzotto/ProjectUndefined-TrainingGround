using CoreGame;
using InteractiveObjects;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPatrolGraphTest", menuName = "Test/AIPatrolGraphTest", order = 1)]
    public class AIPatrolGraphTest : AIPatrolGraphV2
    {
        [GraphPatrolPointAttribute(R = 0f, G = 1f, B = 0f)]
        [GraphPatrolLine(fieldTargetWorldPosition: nameof(AIPatrolGraphTest.P3))]
        public AIMoveToActionInputData P1;

        [GraphPatrolPointAttribute]
        [GraphPatrolLine(fieldTargetWorldPosition: nameof(AIPatrolGraphTest.P1))]
        public AIMoveToActionInputData P2;

        [GraphPatrolPointAttribute]
        [GraphPatrolLine(fieldTargetWorldPosition: nameof(AIPatrolGraphTest.P2))]
        public AIMoveToActionInputData P3;

        public override List<SequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject)
        {
            return new List<SequencedAction>()
            {
                new AIWarpActionV2(InvolvedInteractiveObject, this.TransformToWorldPosition(this.P1.WorldPoint), new List<SequencedAction>(){
                    new BranchInfiniteLoopAction(
                        new List<SequencedAction>(){
                            this.CreateAIMoveToActionV2(InvolvedInteractiveObject, this.P2, new List<SequencedAction>(){
                                this.CreateAIMoveToActionV2(InvolvedInteractiveObject, this.P1, new List<SequencedAction>(){
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
