﻿using AdventureGame;
using CoreGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class POIModuleWizard : BaseModuleWizardModule<PointOfInterestType, APointOfInterestModule>
    {
        protected override List<APointOfInterestModule> GetModules(PointOfInterestType PointOfInterestType)
        {
            return PointOfInterestTypeHelper.GetPointOfInterestModules(PointOfInterestType).GetComponentsInChildren<APointOfInterestModule>().ToList();
        }

        protected override void OnEdit(PointOfInterestType PointOfInterestType, Type selectedType)
        {
            this.POIModuleSwitch(selectedType,
                          PointOfInterestModelObjectModuleAction: () =>
                          {
                              if (this.add)
                              {
                                  EditorPOIModulesOperation.AddModule<PointOfInterestModelObjectModule>(PointOfInterestType);
                              }
                              else
                              {
                                  EditorPOIModulesOperation.RemoveModule<PointOfInterestModelObjectModule>(PointOfInterestType);
                              }
                          },
                          PointOfInterestCutsceneControllerAction: () =>
                          {
                              if (this.add)
                              {
                                  EditorPOIModulesOperation.AddModule<PointOfInterestCutsceneControllerModule>(PointOfInterestType);
                                  EditorPOIModulesOperation.AddDataComponent<TransformMoveManagerComponentV2>(PointOfInterestType);
                              }
                              else
                              {
                                  EditorPOIModulesOperation.RemoveModule<PointOfInterestCutsceneControllerModule>(PointOfInterestType);
                                  EditorPOIModulesOperation.RemoveDataComponent<TransformMoveManagerComponentV2>(PointOfInterestType);
                              }
                          },
                          PointOfInterestVisualMovementModuleAction: () =>
                          {
                              if (this.add)
                              {
                                  this.OnEdit(PointOfInterestType, typeof(PointOfInterestModelObjectModule));
                                  this.OnEdit(PointOfInterestType, typeof(PointOfInterestTrackerModule));
                                  EditorPOIModulesOperation.AddModule<PointOfInterestVisualMovementModule>(PointOfInterestType);
                                  EditorPOIModulesOperation.AddDataComponent<PlayerPOIVisualMovementComponentV2>(PointOfInterestType);
                              }
                              else
                              {
                                  EditorPOIModulesOperation.RemoveModule<PointOfInterestVisualMovementModule>(PointOfInterestType);
                                  EditorPOIModulesOperation.RemoveDataComponent<PlayerPOIVisualMovementComponentV2>(PointOfInterestType);
                              }
                          },
                          PointOfInterestTrackerModuleAction: () =>
                          {
                              if (this.add)
                              {
                                  EditorPOIModulesOperation.AddPrefabModule(PointOfInterestType, this.CommonGameConfigurations.AdventureCommonPrefabs.BasePointOfInterestTrackerModule);
                                  EditorPOIModulesOperation.AddDataComponent<PlayerPOITrackerManagerComponentV2>(PointOfInterestType);
                              }
                              else
                              {
                                  EditorPOIModulesOperation.RemovePrefabModule<PointOfInterestTrackerModule>(PointOfInterestType);
                                  EditorPOIModulesOperation.RemoveDataComponent<PlayerPOITrackerManagerComponentV2>(PointOfInterestType);
                              }
                          }
               );
        }

        protected override string POIModuleDescription(Type selectedType)
        {
            string returnDescription = string.Empty;
            this.POIModuleSwitch(selectedType,
                    PointOfInterestModelObjectModuleAction: () => { returnDescription = "Model object definition."; },
                    PointOfInterestCutsceneControllerAction: () => { returnDescription = "Allow the POI to be controlled by cutscene system."; },
                    PointOfInterestVisualMovementModuleAction: () => { returnDescription = "Allow the POI to animate visual movement over nearest POI."; },
                    PointOfInterestTrackerModuleAction: () => { returnDescription = "Track and store nearest POI."; }
                );
            return returnDescription;
        }


        private void POIModuleSwitch(Type selectedType, Action PointOfInterestModelObjectModuleAction, Action PointOfInterestCutsceneControllerAction,
                Action PointOfInterestVisualMovementModuleAction,
                Action PointOfInterestTrackerModuleAction)
        {
            if (selectedType == typeof(PointOfInterestModelObjectModule))
            {
                PointOfInterestModelObjectModuleAction.Invoke();
            }
            else if (selectedType == typeof(PointOfInterestCutsceneControllerModule))
            {
                PointOfInterestCutsceneControllerAction.Invoke();
            }
            else if (selectedType == typeof(PointOfInterestVisualMovementModule))
            {
                PointOfInterestVisualMovementModuleAction.Invoke();
            }
            else if (selectedType == typeof(PointOfInterestTrackerModule))
            {
                PointOfInterestTrackerModuleAction.Invoke();
            }
        }
    }
}
