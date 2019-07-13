using AdventureGame;
using Editor_MainGameCreationWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public abstract class BaseModuleWizardModule<T, ABSTRACT_MODULE> : IGameDesignerModule where T : UnityEngine.Object
    {
        [NonSerialized]
        protected CommonGameConfigurations CommonGameConfigurations;
        protected GameObject currentSelectedObjet;

        [SerializeField]
        private List<Type> AvailableModules;
        [SerializeField]
        private int selectedModuleIndex;

        [SerializeField]
        protected bool add;
        [SerializeField]
        protected bool remove;

        #region Base Prefabs
        private PointOfInterestTrackerModule BasePOITrackerModule;
        #endregion

        protected abstract void OnEdit(T PointOfInterestType, Type selectedType);
        protected abstract string POIModuleDescription(Type selectedType);
        protected abstract List<ABSTRACT_MODULE> GetModules(T PointOfInterestType);

        public void GUITick()
        {
            if (this.CommonGameConfigurations == null)
            {
                this.CommonGameConfigurations = new CommonGameConfigurations();
                EditorInformationsHelper.InitProperties(ref this.CommonGameConfigurations);
            }

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


            T selectedPointOfIterestType = null;
            if (this.currentSelectedObjet != null)
            {
                selectedPointOfIterestType = this.currentSelectedObjet.GetComponent<T>();
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
            return this.currentSelectedObjet == null || this.currentSelectedObjet.GetComponent<T>() == null;
        }

        private void DoModuleListing(T pointOfInterestType)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("POI Modules : ");
            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

            var foundedModules = this.GetModules(pointOfInterestType);

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
            this.AvailableModules = TypeHelper.GetAllTypeAssignableFrom(typeof(ABSTRACT_MODULE)).ToList();
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

    }
}
