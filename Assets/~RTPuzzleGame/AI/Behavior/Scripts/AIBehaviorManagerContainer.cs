using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class AIBehaviorManagerContainer
    {

        private SortedList<int, InterfaceAIManager> aIManagersByExecutionOrder;

        public SortedList<int, InterfaceAIManager> AIManagersByExecutionOrder { get => aIManagersByExecutionOrder; }

        public AIBehaviorManagerContainer(SortedList<int, InterfaceAIManager> aIManagersByExecutionOrder)
        {
            this.aIManagersByExecutionOrder = aIManagersByExecutionOrder;
        }
        
        public int GetAIManagerIndex(in InterfaceAIManager aiManager)
        {
            return this.aIManagersByExecutionOrder.Keys[this.aIManagersByExecutionOrder.IndexOfValue(aiManager)];
        }
    }

}
