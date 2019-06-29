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

            this.add = EditorGUILayout.Toggle(new GUIContent("ADD"), this.add, EditorStyles.miniButtonLeft);
            this.remove = EditorGUILayout.Toggle(new GUIContent("REMOVE"), this.remove, EditorStyles.miniButtonRight);

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
            if (selectedType == typeof(PointOfInterestModelObjectModule))
            {
                if (this.add)
                {
                    EditorPOIModulesOperation.AddModule<PointOfInterestModelObjectModule>(PointOfInterestType);
                }
                else
                {
                    EditorPOIModulesOperation.RemoveModule<PointOfInterestModelObjectModule>(PointOfInterestType);
                }
            }
            else if (selectedType == typeof(PointOfInterestCutsceneController))
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
            }
            else if (selectedType == typeof(PointOfInterestSpecificBehaviorModule))
            {
                if (this.add)
                {
                    EditorPOIModulesOperation.AddModule<PointOfInterestSpecificBehaviorModule>(PointOfInterestType);
                }
                else
                {
                    EditorPOIModulesOperation.RemoveModule<PointOfInterestSpecificBehaviorModule>(PointOfInterestType);
                }
            }
            else if (selectedType == typeof(PointOfInterestVisualMovementModule))
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
            }
            else if (selectedType == typeof(PointOfInterestTrackerModule))
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
                    if (GUILayout.Button(foundedModule.GetType().Name))
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
    }
}
