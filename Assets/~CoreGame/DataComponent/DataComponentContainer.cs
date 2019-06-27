using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class DataComponentContainer : MonoBehaviour
    {
        private Dictionary<Type, ADataComponent> dataComponents;

        public void Init()
        {
            this.dataComponents = new Dictionary<Type, ADataComponent>();
            var retrievedDataComponents = GetComponentsInChildren<ADataComponent>();
            if (retrievedDataComponents != null)
            {
                foreach (var retrievedDataComponent in retrievedDataComponents)
                {
                    this.dataComponents[retrievedDataComponent.GetType()] = retrievedDataComponent;
                }
            }
        }

        public T GetDataComponent<T>() where T : ADataComponent
        {
            return (T)this.dataComponents[typeof(T)];
        }
    }

}

