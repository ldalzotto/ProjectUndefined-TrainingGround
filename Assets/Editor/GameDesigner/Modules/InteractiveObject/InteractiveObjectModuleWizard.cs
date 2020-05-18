﻿using ConfigurationEditor;
using CreationWizard;
using Editor_AttractiveObjectCreationWizard;
using Editor_MainGameCreationWizard;
using Editor_RepelableObjectCreationWizard;
using Editor_TargetZoneCreationWizard;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class InteractiveObjectModuleWizard : BaseModuleWizardModule<InteractiveObjectType, InteractiveObjectModule>
    {
        [SerializeField]
        private bool deepDeletion = false;

        protected override List<InteractiveObjectModule> GetModules(InteractiveObjectType RootModuleObject)
        {
            return RootModuleObject.GetComponentsInChildren<InteractiveObjectModule>().ToList();
        }

        protected override void OnEdit(InteractiveObjectType InteractiveObjectType, Type selectedType)
        {
            this.InteractiveObjectModuleSwitch(selectedType,
               InteractiveObjectModuleAction: () =>
               {
                   if (this.add)
                   {
                       EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseModelObjectModule);
                   }
                   else if (this.remove)
                   {
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<ModelObjectModule>(InteractiveObjectType);
                   }
               },
               ObjectRepelTypeModuleAction: () =>
               {
                   if (this.add)
                   {
                       var objectRepelTypeModule = EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseObjectRepelTypeModule);
                       objectRepelTypeModule.RepelableObjectID = this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.RepelableObjectID;
                       EditorUtility.SetDirty(this.currentSelectedObjet);
                   }
                   else if (this.remove)
                   {
                       if (this.deepDeletion)
                       {
                           //Get ID
                           var RetrievedObjectRepelTypeModule = InteractiveObjectType.GetComponentInChildren<ObjectRepelTypeModule>();
                           if (RetrievedObjectRepelTypeModule != null)
                           {
                               var ojectRepelID = RetrievedObjectRepelTypeModule.RepelableObjectID;

                               //configuration deletion
                               AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this.CommonGameConfigurations.PuzzleGameConfigurations.RepelableObjectsConfiguration.ConfigurationInherentData[ojectRepelID]));
                               this.CommonGameConfigurations.PuzzleGameConfigurations.RepelableObjectsConfiguration.ClearEntry(ojectRepelID);
                           }
                       }
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<ObjectRepelTypeModule>(InteractiveObjectType);
                   }
               },
               AttractiveObjectTypeAction: () =>
               {
                   if (this.add)
                   {
                       var attractiveObjectTypeModule = EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseAttractiveObjectModule);
                       attractiveObjectTypeModule.AttractiveObjectId = this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.AttractiveObjectId;
                       EditorUtility.SetDirty(this.currentSelectedObjet);

                       var configurationToModify = this.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration.ConfigurationInherentData[this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.AttractiveObjectId];
                       configurationToModify.AttractiveInteractiveObjectPrefab = AssetDatabase.LoadAssetAtPath<InteractiveObjectType>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(InteractiveObjectType));
                       EditorUtility.SetDirty(configurationToModify);
                   }
                   else if (this.remove)
                   {
                       if (this.deepDeletion)
                       {
                           //Get ID
                           var AttractiveObjectTypeModule = InteractiveObjectType.GetComponentInChildren<AttractiveObjectTypeModule>();
                           if (AttractiveObjectTypeModule != null)
                           {
                               var AttractiveObjectId = AttractiveObjectTypeModule.AttractiveObjectId;

                               //configuration deletion
                               AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration.ConfigurationInherentData[AttractiveObjectId]));
                               this.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration.ClearEntry(AttractiveObjectId);
                           }
                       }
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<AttractiveObjectTypeModule>(InteractiveObjectType);
                   }
               },
               TargetZoneObjectModuleAction: () =>
               {
                   if (this.add)
                   {
                       var targetZoneObjectModule = EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseTargetZoneObjectModule);
                       targetZoneObjectModule.TargetZoneID = this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.TargetZoneID;
                       EditorUtility.SetDirty(this.currentSelectedObjet);
                   }
                   else if (this.remove)
                   {
                       if (this.deepDeletion)
                       {
                           //Get ID
                           var TargetZoneModule = InteractiveObjectType.GetComponentInChildren<TargetZoneObjectModule>();
                           if (TargetZoneModule != null)
                           {
                               var TargetZoneID = TargetZoneModule.TargetZoneID;

                               //configuration deletion
                               AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this.CommonGameConfigurations.PuzzleGameConfigurations.TargetZonesConfiguration.ConfigurationInherentData[TargetZoneID]));
                               this.CommonGameConfigurations.PuzzleGameConfigurations.TargetZonesConfiguration.ClearEntry(TargetZoneID);
                           }
                       }
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<TargetZoneObjectModule>(InteractiveObjectType);
                   }
               },
               LevelCompletionTriggerAction: () =>
               {
                   if (this.add)
                   {
                       EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseLevelCompletionTriggerModule);
                   }
                   else if (this.remove)
                   {
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<LevelCompletionTriggerModule>(InteractiveObjectType);
                   }
               }
             );
        }

        private void PrefabEditConditionWithID<CREATION_WIZARD>(SerializedProperty idSerializedProperty, Enum idProfileField, IConfigurationSerialization configuration, ref bool allowedToEdit)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(idSerializedProperty);
            if (GUILayout.Button("CREATE ID BASED ON OBJECT NAME"))
            {
                string preFilledName = this.currentSelectedObjet.name.Substring(0, this.currentSelectedObjet.name.LastIndexOf("_"));
                EnumIDGeneration.Init(idProfileField.GetType(), preFilledName);
            }
            string isError = ErrorHelper.NotAlreadyPresentInConfiguration(idProfileField, configuration.GetKeys(), nameof(configuration));
            if (!string.IsNullOrEmpty(isError))
            {
                EditorGUILayout.HelpBox(isError, MessageType.Error);
                if (GUILayout.Button("CREATE IN EDITOR"))
                {
                    GameCreationWizard.InitWithSelected(typeof(CREATION_WIZARD).Name);
                }
            }
            else
            {
                allowedToEdit = true;
            }

            if (this.remove)
            {
                this.deepDeletion = GUILayout.Toggle(this.deepDeletion, "Deep deletion", EditorStyles.miniButton, GUILayout.Width(150f));
            }

            EditorGUILayout.Separator();
        }

        protected override bool AdditionalEditCondition(Type selectedType)
        {
            bool allowedToEdit = false;
            this.InteractiveObjectModuleSwitch(selectedType,
                    InteractiveObjectModuleAction: () => allowedToEdit = true,
                    ObjectRepelTypeModuleAction: () =>
                    {
                        this.PrefabEditConditionWithID<RepelableObjectCreationWizard>(this.GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID.RepelableObjectID"),
                            this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.RepelableObjectID,
                            this.CommonGameConfigurations.PuzzleGameConfigurations.RepelableObjectsConfiguration, ref allowedToEdit);
                    },
                    AttractiveObjectTypeAction: () =>
                    {
                        this.PrefabEditConditionWithID<AttractiveObjectCreationWizard>(this.GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID.AttractiveObjectId"),
                            this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.AttractiveObjectId,
                            this.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration, ref allowedToEdit);
                    },
                    TargetZoneObjectModuleAction: () =>
                    {
                        this.PrefabEditConditionWithID<TargetZoneCreationWizard>(this.GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID.TargetZoneID"),
                           this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.TargetZoneID,
                           this.CommonGameConfigurations.PuzzleGameConfigurations.TargetZonesConfiguration, ref allowedToEdit);
                    },
                    LevelCompletionTriggerAction: () => allowedToEdit = true
                );
            return allowedToEdit;
        }

        protected override string POIModuleDescription(Type selectedType)
        {
            string returnDescription = string.Empty;
            this.InteractiveObjectModuleSwitch(selectedType,
                    InteractiveObjectModuleAction: () => { returnDescription = "Model object definition."; },
                    ObjectRepelTypeModuleAction: () => { returnDescription = "Allow the object to be repelled by projectile."; },
                    AttractiveObjectTypeAction: () => { returnDescription = "Attract AI when on range."; },
                    TargetZoneObjectModuleAction: () => { returnDescription = "Definie a zone for AI to reach to complete level."; },
                    LevelCompletionTriggerAction: () => { returnDescription = "Trigger Zone used to trigger end of puzzle level event."; }
                );
            return returnDescription;
        }

        private void InteractiveObjectModuleSwitch(Type selectedType, Action InteractiveObjectModuleAction, Action ObjectRepelTypeModuleAction, Action AttractiveObjectTypeAction, Action TargetZoneObjectModuleAction,
                Action LevelCompletionTriggerAction)
        {
            if (selectedType == typeof(ModelObjectModule))
            {
                InteractiveObjectModuleAction.Invoke();
            }
            else if (selectedType == typeof(ObjectRepelTypeModule))
            {
                ObjectRepelTypeModuleAction.Invoke();
            }
            else if (selectedType == typeof(AttractiveObjectTypeModule))
            {
                AttractiveObjectTypeAction.Invoke();
            }
            else if (selectedType == typeof(TargetZoneObjectModule))
            {
                TargetZoneObjectModuleAction.Invoke();
            }
            else if (selectedType == typeof(LevelCompletionTriggerModule))
            {
                LevelCompletionTriggerAction.Invoke();
            }
        }
    }
}
