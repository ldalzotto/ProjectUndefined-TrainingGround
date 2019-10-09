using CoreGame;
using InteractiveObjectTest;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPatrolGraphTest", menuName = "Test/AIPatrolGraphTest", order = 1)]
    public class AIPatrolGraphTest : AIPatrolGraphV2
    {
        public TransformStruct P1;
        public TransformStruct P2;
        
        public override List<SequencedAction> AIPatrolGraphActions()
        {
            return new List<SequencedAction>()
            {
                new AIWarpActionV2(this.CoreInteractiveObject, this.TrasnformToWorldPosition(this.P1), new List<SequencedAction>(){
                    new BranchInfiniteLoopAction(
                          new List<SequencedAction>(){ new AIMoveToActionV2(this.CoreInteractiveObject, this.TrasnformToWorldPosition(this.P2) ,
                                    new List<SequencedAction>(){ new AIMoveToActionV2(this.CoreInteractiveObject, this.TrasnformToWorldPosition(this.P1), null) })
                          })
                })
            };
        }
    }

}
