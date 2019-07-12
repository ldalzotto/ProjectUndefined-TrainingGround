using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class AIBehaviorManagerContainer
    {

        private SortedList<int, InterfaceAIManager> aIManagersByExecutionOrder;
        private Dictionary<Type, InterfaceAIManager> aIManagersManagerType;

        public SortedList<int, InterfaceAIManager> AIManagersByExecutionOrder { get => aIManagersByExecutionOrder; }

        public AIBehaviorManagerContainer(SortedList<int, InterfaceAIManager> aIManagersByExecutionOrder)
        {
            this.aIManagersByExecutionOrder = aIManagersByExecutionOrder;

            this.aIManagersManagerType = new Dictionary<Type, InterfaceAIManager>();
            foreach (var aiManager in this.aIManagersByExecutionOrder.Values)
            {
                this.aIManagersManagerType.Add(aiManager.GetType(), aiManager);
            }
        }

        public T GetManager<T>(AbstractAIComponent abstractAIComponent) where T : InterfaceAIManager
        {
            return (T)aIManagersManagerType[abstractAIComponent.SelectedManagerType];
        }

        public int GetAIManagerIndex(in InterfaceAIManager aiManager)
        {
            return this.aIManagersByExecutionOrder.Keys[this.aIManagersByExecutionOrder.IndexOfValue(aiManager)];
        }

        public IReadOnlyCollection<InterfaceAIManager> GetAllAIManagers()
        {
            return this.aIManagersManagerType.Values;
        }
        
    }

}
