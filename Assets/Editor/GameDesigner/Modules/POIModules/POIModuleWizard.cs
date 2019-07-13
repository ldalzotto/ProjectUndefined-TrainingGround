using AdventureGame;
using CoreGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class POIModuleWizard : IGameDesignerModule
    {
        protected GameObject currentSelectedObjet;

        [SerializeField]
        private List<Type> AvailableModules;
        [SerializeField]
        private int selectedModuleIndex;

        [SerializeField]
        private bool add;
        [SerializeField]
        private bool remove;

        #region Base Prefabs
        private PointOfInterestTrackerModule BasePOITrackerModule;
        #endregion

        public void GUITick()
        {
            this.currentSelectedObjet = GameDesignerHelper.GetCurrentSceneSelectedObject();
            this.selectedModuleIndex = EditorGUILayout.Popup(this.selectedModuleIndex, this.AvailableModules.ConvertAll(t => t.Name).ToArray());

            EditorGUILayout.BeginHorizontal();
            bool newAdd = GUILayout.Toggle(this.add, "ADD", EditorStyles.miniButtonLeft);
            bool newRemove = GUILayout.Toggle(this.remove, "REMOVE", EditorStyles.miniButtonRight);

            if (newAdd && newRemove)
            {
                if (this.add && !this.remove)
                {
                    this.add = false;
                    this.remove = true;
                }
                else if (!this.add && this.remove)
                {
                    this.add = true;
                    this.remove = false;
                }
                else
                {
                    this.add = newAdd;
                    this.remove = newRemove;
                }
            }
            else
            {
                this.add = newAdd;
                this.remove = newRemove;
            }

            EditorGUILayout.EndHorizontal();


            PointOfInterestType selectedPointOfIterestType = null;
            if (this.currentSelectedObjet != null)
            {
                selectedPointOfIterestType = this.currentSelectedObjet.GetComponent<PointOfInterestType>();
            }

            EditorGUI.BeginDisabledGroup(this.IsDisabled());
            if (GUILayout.Button("EDIT"))
            {
                this.OnEnabled();
                this.OnEdit(selectedPointOfIterestType, this.AvailableModules[this.selectedModuleIndex]);
                EditorUtility.SetDirty(this.currentSelectedObjet);
            }
            EditorGUI.EndDisabledGroup();

            if (this.currentSelectedObjet != null && selectedPointOfIterestType != null)
            {
                this.DoModuleListing(selectedPointOfIterestType);
            }

        }

        private bool IsDisabled()
        {
            return this.currentSelectedObjet == null || this.currentSelectedObjet.GetComponent<PointOfInterestType>() == null;
        }

        private void OnEdit(PointOfInterestType PointOfInterestType, Type selectedType)
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
                         EditorPOIModulesOperation.AddModule<PointOfInterestCutsceneController>(PointOfInterestType);
                         EditorPOIModulesOperation.AddDataComponent<TransformMoveManagerComponentV2>(PointOfInterestType);
                     }
                     else
                     {
                         EditorPOIModulesOperation.RemoveModule<PointOfInterestCutsceneController>(PointOfInterestType);
                         EditorPOIModulesOperation.RemoveDataComponent<TransformMoveManagerComponentV2>(PointOfInterestType);
                     }
                 },
                 PointOfInterestSpecificBehaviorModuleAction: () =>
                 {
                     if (this.add)
                     {
                         EditorPOIModulesOperation.AddModule<PointOfInterestSpecificBehaviorModule>(PointOfInterestType);
                     }
                     else
                     {
                         EditorPOIModulesOperation.RemoveModule<PointOfInterestSpecificBehaviorModule>(PointOfInterestType);
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
                         EditorPOIModulesOperation.AddPrefabModule(PointOfInterestType, this.BasePOITrackerModule);
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

        private void DoModuleListing(PointOfInterestType pointOfInterestType)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("POI Modules : ");
            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

            var foundedModules = PointOfInterestTypeHelper.GetPointOfInterestModules(pointOfInterestType).GetComponentsInChildren<APointOfInterestModule>();

            EditorGUILayout.BeginVertical();
            if (foundedModules != null)
            {
                foreach (var foundedModule in foundedModules)
                {
                    if (GUILayout.Button(new GUIContent(foundedModule.GetType().Name, this.POIModuleDescription(foundedModule.GetType()))))
                    {
                        this.selectedModuleIndex = this.AvailableModules.IndexOf(foundedModule.GetType());
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        [SerializeField]
        private Vector2 scrollPosition;

        public void OnEnabled()
        {
            this.AvailableModules = TypeHelper.GetAllTypeAssignableFrom(typeof(APointOfInterestModule)).ToList();
            this.BasePOITrackerModule = AssetFinder.SafeSingleAssetFind<PointOfInterestTrackerModule>("BasePOITrackerModule");
        }

        public void OnDisabled()
        {
        }

        private void POIModuleSwitch(Type selectedType, Action PointOfInterestModelObjectModuleAction, Action PointOfInterestCutsceneControllerAction,
            Action PointOfInterestSpecificBehaviorModuleAction, Action PointOfInterestVisualMovementModuleAction,
            Action PointOfInterestTrackerModuleAction)
        {
            if (selectedType == typeof(PointOfInterestModelObjectModule))
            {
                PointOfInterestModelObjectModuleAction.Invoke();
            }
            else if (selectedType == typeof(PointOfInterestCutsceneController))
            {
                PointOfInterestCutsceneControllerAction.Invoke();
            }
            else if (selectedType == typeof(PointOfInterestSpecificBehaviorModule))
            {
                PointOfInterestSpecificBehaviorModuleAction.Invoke();
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

        private string POIModuleDescription(Type selectedType)
        {
            string returnDescription = string.Empty;
            this.POIModuleSwitch(selectedType,
                    PointOfInterestModelObjectModuleAction: () => { returnDescription = "Model object definition."; },
                    PointOfInterestCutsceneControllerAction: () => { returnDescription = "Allow the POI to be controlled by cutscene system."; },
                    PointOfInterestSpecificBehaviorModuleAction: () => { returnDescription = "Adding specific behavior to POI."; },
                    PointOfInterestVisualMovementModuleAction: () => { returnDescription = "Allow the POI to animate visual movement over nearest POI."; },
                    PointOfInterestTrackerModuleAction: () => { returnDescription = "Track and store nearest POI."; }
                );
            return returnDescription;
        }
    }
}
