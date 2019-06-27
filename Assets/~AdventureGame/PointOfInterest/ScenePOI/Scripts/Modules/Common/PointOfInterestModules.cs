using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestModules : MonoBehaviour
    {
        private Dictionary<Type, APointOfInterestModule> pointOfInterestModules;

        #region Data Retrieval
        public Dictionary<Type, APointOfInterestModule> GetAllPointOfInterestModules()
        {
            return this.pointOfInterestModules;
        }
        public T GetModule<T>() where T : APointOfInterestModule
        {
            return (T)this.pointOfInterestModules[typeof(T)];
        }
        #endregion

        public void Init(PointOfInterestType pointOfInterestTypeRef)
        {
            this.pointOfInterestModules = new Dictionary<Type, APointOfInterestModule>();
            var retrievedPointOfInterestModules = this.transform.GetComponentsInChildren<APointOfInterestModule>();
            if (retrievedPointOfInterestModules != null)
            {
                foreach (var retrievedPointOfInterestModule in retrievedPointOfInterestModules)
                {
                    this.pointOfInterestModules[retrievedPointOfInterestModule.GetType()] = retrievedPointOfInterestModule;
                    this.pointOfInterestModules[retrievedPointOfInterestModule.GetType()].Init(pointOfInterestTypeRef, this);
                }
            }
        }

        public void Tick(float d)
        {
            foreach (var pointOfInterestModule in this.pointOfInterestModules)
            {
                pointOfInterestModule.Value.Tick(d);
            }
        }

    }

}
