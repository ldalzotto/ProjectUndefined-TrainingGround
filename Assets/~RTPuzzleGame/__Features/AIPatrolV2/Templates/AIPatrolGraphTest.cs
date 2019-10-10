using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPatrolGraphTest", menuName = "Test/AIPatrolGraphTest", order = 1)]
    public class AIPatrolGraphTest : AIPatrolGraphV2
    {
        public AIMoveToActionInputData P1;
        public AIMoveToActionInputData P2;
        public AIMoveToActionInputData P3;

        public override List<SequencedAction> AIPatrolGraphActions()
        {
            return new List<SequencedAction>()
            {
                new AIWarpActionV2(this.InvolvedInteractiveObject, this.TransformToWorldPosition(this.P1.WorldPoint), new List<SequencedAction>(){
                    new BranchInfiniteLoopAction(
                        new List<SequencedAction>(){
                            this.CreateAIMoveToActionV2(this.InvolvedInteractiveObject, this.P2, new List<SequencedAction>(){
                                this.CreateAIMoveToActionV2(this.InvolvedInteractiveObject, this.P1, new List<SequencedAction>(){
                                    this.CreateAIMoveToActionV2(this.InvolvedInteractiveObject, this.P3, null)
                                })
                            })
                        }
                    )
                })
            };
        }
    }
}
