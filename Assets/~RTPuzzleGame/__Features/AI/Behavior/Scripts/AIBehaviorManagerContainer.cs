using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class AIBehaviorManagerContainer
    {

        private SortedList<int, InterfaceAIManager> aIManagersByExecutionOrder;
        private Dictionary<Type, InterfaceAIManager> aIManagers;

        public SortedList<int, InterfaceAIManager> AIManagersByExecutionOrder { get => aIManagersByExecutionOrder; }
        public Dictionary<Type, InterfaceAIManager> AIManagers { get => aIManagers; }

        public AIBehaviorManagerContainer(List<InterfaceAIManager> aiManagers)
        {
            this.aIManagers = new Dictionary<Type, InterfaceAIManager>();
            foreach (var aiManager in aiManagers)
            {
                if (aiManager != null)
                {
                    this.aIManagers[aiManager.GetType().BaseType] = aiManager;
                }
            }
        }

        public void SetAIManagersByExecutionOrder(SortedList<int, InterfaceAIManager> aIManagersByExecutionOrder)
        {
            this.aIManagersByExecutionOrder = aIManagersByExecutionOrder;
        }

        public int GetAIManagerIndex(in InterfaceAIManager aiManager)
        {
            return this.aIManagersByExecutionOrder.Keys[this.aIManagersByExecutionOrder.IndexOfValue(aiManager)];
        }

        public InterfaceAIManager GetAIManager<T>() where T : InterfaceAIManager
        {
            this.aIManagers.TryGetValue(typeof(T), out InterfaceAIManager AIManager);
            return AIManager;
        }
    }

}
