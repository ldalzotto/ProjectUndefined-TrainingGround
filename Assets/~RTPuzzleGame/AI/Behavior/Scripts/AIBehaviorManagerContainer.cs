using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IReadOnlyCollection<InterfaceAIManager> GetAllAIManagers()
        {
            return this.aIManagersManagerType.Values;
        }

        public bool EvaluateAIManagerAvailabilityToTheFirst(in InterfaceAIManager aiManager, EvaluationType evaluationType = EvaluationType.INCLUDED)
        {
            var startIndex = this.aIManagersByExecutionOrder.IndexOfValue(aiManager);
            if (evaluationType == EvaluationType.EXCLUDED)
            {
                startIndex -= 1;
            }
            if (startIndex >= 0)
            {
                for (var i = startIndex; i >= 0; i--)
                {
                    if (aIManagersByExecutionOrder.Values[i].IsManagerEnabled())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public enum EvaluationType
        {
            INCLUDED, EXCLUDED
        }

    }

}
