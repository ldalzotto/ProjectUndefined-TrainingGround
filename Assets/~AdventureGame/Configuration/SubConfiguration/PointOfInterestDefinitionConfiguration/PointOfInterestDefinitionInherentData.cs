//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdventureGame
{
    using GameConfigurationID;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using static AdventureGame.PointOfInterestTrackerModule;

    [System.Serializable()]
    [UnityEngine.CreateAssetMenu(fileName = "PointOfInterestDefinitionInherentData", menuName = "Configuration/AdventureGame/PointOfInterestDefinitionConfiguration/PointOfInteres" +
        "tDefinitionInherentData", order = 1)]
    public class PointOfInterestDefinitionInherentData : AbstractObjectDefinitionConfigurationInherentData
    {
        public PointOfInterestDefinitionInherentData() { }

        public override List<Type> ModuleTypes => PointOfInterestModuleTypesConstants.PointOfInterestModuleTypes;

        [CustomEnum()]
        public PointOfInterestId PointOfInterestId;

        [Inline(createAtSameLevelIfAbsent: true)]
        public PointOfInterestSharedDataTypeInherentData PointOfInterestSharedDataTypeInherentData;

        public void DefinePointOfInterest(PointOfInterestType PointOfInterestType, AdventurePrefabConfiguration AdventurePrefabConfiguration)
        {
            PointOfInterestType.PointOfInterestId = PointOfInterestId;

            if (this.RangeDefinitionModulesActivation != null && this.RangeDefinitionModules != null)
            {
                foreach (var rangeDefinitionModuleActivation in this.RangeDefinitionModulesActivation)
                {
                    if (rangeDefinitionModuleActivation.Value)
                    {
                        var moduleConfiguration = this.RangeDefinitionModules[rangeDefinitionModuleActivation.Key];

                        if (moduleConfiguration.GetType() == typeof(PointOfInterestCutsceneControllerModuleDefinition))
                        {
                            var PointOfInterestCutsceneControllerModuleDefinition = (PointOfInterestCutsceneControllerModuleDefinition)moduleConfiguration;
                            var PointOfInterestCutsceneControllerModule = MonoBehaviour.Instantiate(AdventurePrefabConfiguration.BasePointOfInterestCutsceneControllerModule, PointOfInterestType.transform);
                            this.EnableNavMeshAgent(PointOfInterestType);
                        }
                        else if (moduleConfiguration.GetType() == typeof(PointOfInterestTrackerModuleDefinition))
                        {
                            var PointOfInterestTrackerModuleDefinition = (PointOfInterestTrackerModuleDefinition)moduleConfiguration;
                            var PointOfInterestTrackerModule = MonoBehaviour.Instantiate(AdventurePrefabConfiguration.BasePointOfInterestTrackerModule, PointOfInterestType.transform);
                            PointOfInterestTrackerModuleInstancer.PopuplateFromDefinition(PointOfInterestTrackerModule, PointOfInterestTrackerModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(PointOfInterestVisualMovementModuleDefinition))
                        {
                            var PointOfInterestVisualMovementModuleDefinition = (PointOfInterestVisualMovementModuleDefinition)moduleConfiguration;
                            var PointOfInterestVisualMovementModule = MonoBehaviour.Instantiate(AdventurePrefabConfiguration.BasePointOfInterestVisualMovementModule, PointOfInterestType.transform);
                        }
                        else if (moduleConfiguration.GetType() == typeof(PointOfInterestModelObjectModuleDefinition))
                        {
                            var PointOfInterestModelObjectModuleDefinition = (PointOfInterestModelObjectModuleDefinition)moduleConfiguration;
                            var PointOfInterestModelObjectModule = MonoBehaviour.Instantiate(AdventurePrefabConfiguration.BasePointOfInterestModelObjectModule, PointOfInterestType.transform);
                            var modelObject = GameObject.Instantiate(PointOfInterestModelObjectModuleDefinition.ModelObject, PointOfInterestModelObjectModule.transform);
                            modelObject.transform.localPosition = Vector3.zero;
                            modelObject.transform.localRotation = Quaternion.identity;
                        }
                    }
                }
            }
        }

        private void EnableNavMeshAgent(PointOfInterestType PointOfInterestType)
        {
            if (PointOfInterestType.PointOfInterestDefinitionID != PointOfInterestDefinitionID.PLAYER)
            {
                PointOfInterestType.transform.GetComponent<NavMeshAgent>().enabled = true;
            }
        }
    }
}
