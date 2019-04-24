using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class AIBehaviorManagerContainer
    {

        private Dictionary<int, InterfaceAIManager> aIManagersByExecutionOrder;
        private Dictionary<Type, InterfaceAIManager> aIManagersManagerType;

        public AIBehaviorManagerContainer(Dictionary<int, InterfaceAIManager> aIManagersByExecutionOrder)
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

        public IReadOnlyCollection<InterfaceAIManager> GetAllAIManagers()
        {
            return this.aIManagersManagerType.Values;
        }
    }

}
