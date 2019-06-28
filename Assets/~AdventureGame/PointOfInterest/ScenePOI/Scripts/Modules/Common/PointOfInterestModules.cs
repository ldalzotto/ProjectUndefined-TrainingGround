using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestModules : MonoBehaviour
    {
        private Dictionary<Type, APointOfInterestModule> pointOfInterestModules;

        public static Dictionary<Type, int> PointOfInterestModulesInitialisationOrder = new Dictionary<Type, int>()
        {
            {typeof(PointOfInterestModelObjectModule), 0 },
            {typeof(PointOfInterestCutsceneController), 1 },
            {typeof(PointOfInterestSpecificBehaviorModule), 2 }
        };

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
                Array.Sort(retrievedPointOfInterestModules, new Comparison<APointOfInterestModule>(
                      (i1, i2) => PointOfInterestModules.PointOfInterestModulesInitialisationOrder[i1.GetType()].CompareTo(PointOfInterestModules.PointOfInterestModulesInitialisationOrder[i2.GetType()])
                    ));
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
