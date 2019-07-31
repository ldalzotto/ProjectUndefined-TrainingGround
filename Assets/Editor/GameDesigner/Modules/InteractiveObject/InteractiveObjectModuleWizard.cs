using ConfigurationEditor;
using CreationWizard;
using Editor_ActionInteractableObjectCreationWizard;
using Editor_AttractiveObjectCreationWizard;
using Editor_DisarmObjectCreationWizard;
using Editor_MainGameCreationWizard;
using Editor_ProjectileCreationWizard;
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
                       configurationToModify.AttractiveInteractiveObjectPrefab = AssetFinder.SafeSingleAssetFind<InteractiveObjectType>(InteractiveObjectType.gameObject.name);
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
               },
               LaunchProjectileModuleAction: () =>
               {
                   if (this.add)
                   {
                       var launchProjectileModule = EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseLaunchProjectileModule);
                       launchProjectileModule.LaunchProjectileId = this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.LaunchProjectileId;

                       var configurationToModify = this.CommonGameConfigurations.PuzzleGameConfigurations.ProjectileConfiguration.ConfigurationInherentData[this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.LaunchProjectileId];
                       configurationToModify.ProjectilePrefabV2 = AssetFinder.SafeSingleAssetFind<InteractiveObjectType>(InteractiveObjectType.gameObject.name);
                       EditorUtility.SetDirty(configurationToModify);
                   }
                   else if (this.remove)
                   {
                       if (this.deepDeletion)
                       {
                           var launchProjectileId = this.currentSelectedObjet.GetComponentInChildren<LaunchProjectileModule>().LaunchProjectileId;
                           //configuration deletion
                           AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this.CommonGameConfigurations.PuzzleGameConfigurations.ProjectileConfiguration.ConfigurationInherentData[launchProjectileId]));
                           this.CommonGameConfigurations.PuzzleGameConfigurations.ProjectileConfiguration.ClearEntry(launchProjectileId);
                       }
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<LaunchProjectileModule>(InteractiveObjectType);
                   }
               },
               DisarmObjectModuleAction: () =>
               {
                   if (this.add)
                   {
                       var disarmObjectModule = EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseDisarmObjectModule);
                       disarmObjectModule.DisarmObjectID = this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.DisarmObjectID;

                       var configurationToModify = this.CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration.ConfigurationInherentData[this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.DisarmObjectID];
                       configurationToModify.DisarmObjectPrefab = AssetFinder.SafeSingleAssetFind<InteractiveObjectType>(InteractiveObjectType.gameObject.name);
                       EditorUtility.SetDirty(configurationToModify);
                   }
                   else if (this.remove)
                   {
                       if (this.deepDeletion)
                       {
                           var disarmObjectID = this.currentSelectedObjet.GetComponentInChildren<DisarmObjectModule>().DisarmObjectID;
                           //configuration deletion
                           AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this.CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration.ConfigurationInherentData[disarmObjectID]));
                           this.CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration.ClearEntry(disarmObjectID);
                       }
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<LaunchProjectileModule>(InteractiveObjectType);
                   }
               },
               ActionInteractableObjectModuleAction: () =>
               {
                   if (this.add)
                   {
                       var actionInteractableObjectModule = EditorInteractiveObjectModulesOperation.AddPrefabModule(InteractiveObjectType, this.CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseActionInteractableObjectModule);
                       actionInteractableObjectModule.ActionInteractableObjectID = this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.ActionInteractableObjectID;

                       var configurationToModify = this.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration.ConfigurationInherentData[this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.ActionInteractableObjectID];
                       configurationToModify.ActionInteractableObjectPrefab = AssetFinder.SafeSingleAssetFind<InteractiveObjectType>(InteractiveObjectType.gameObject.name);
                       EditorUtility.SetDirty(configurationToModify);
                   }
                   else if (this.remove)
                   {
                       if (this.deepDeletion)
                       {
                           var ActionInteractableObjectID = this.currentSelectedObjet.GetComponentInChildren<ActionInteractableObjectModule>().ActionInteractableObjectID;
                           //configuration deletion
                           AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration.ConfigurationInherentData[ActionInteractableObjectID]));
                           this.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration.ClearEntry(ActionInteractableObjectID);
                       }
                       EditorInteractiveObjectModulesOperation.RemovePrefabModule<LaunchProjectileModule>(InteractiveObjectType);
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
                    LevelCompletionTriggerAction: () => allowedToEdit = true,
                    LaunchProjectileModuleAction: () =>
                    {
                        this.PrefabEditConditionWithID<ProjectileCreationWizard>(this.GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID.LaunchProjectileId"),
                            this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.LaunchProjectileId,
                            this.CommonGameConfigurations.PuzzleGameConfigurations.ProjectileConfiguration, ref allowedToEdit);
                    },
                    DisarmObjectModuleAction: () =>
                    {
                        this.PrefabEditConditionWithID<DisarmObjectCreationWizard>(this.GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID.DisarmObjectID"),
                                                   this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.DisarmObjectID,
                                                   this.CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration, ref allowedToEdit);
                    },
                    ActionInteractableObjectModuleAction: () =>
                    {
                        this.PrefabEditConditionWithID<ActionInteractableObjectCreationWizard>(this.GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID.ActionInteractableObjectID"),
                                                  this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.ActionInteractableObjectID,
                                                  this.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration, ref allowedToEdit);
                    }
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
                    LevelCompletionTriggerAction: () => { returnDescription = "Trigger Zone used to trigger end of puzzle level event."; },
                    LaunchProjectileModuleAction: () => { returnDescription = "Projectile following a path and do action on contact."; },
                    DisarmObjectModuleAction: () => { returnDescription = "The object can be deleted by AI."; },
                    ActionInteractableObjectModuleAction: () => { returnDescription = "The object can be interacte by player."; }
                );
            return returnDescription;
        }

        private void InteractiveObjectModuleSwitch(Type selectedType, Action InteractiveObjectModuleAction, Action ObjectRepelTypeModuleAction, Action AttractiveObjectTypeAction, Action TargetZoneObjectModuleAction,
                Action LevelCompletionTriggerAction, Action LaunchProjectileModuleAction, Action DisarmObjectModuleAction, Action ActionInteractableObjectModuleAction)
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
            else if (selectedType == typeof(LaunchProjectileModule))
            {
                LaunchProjectileModuleAction.Invoke();
            }
            else if (selectedType == typeof(DisarmObjectModule))
            {
                DisarmObjectModuleAction.Invoke();
            }
            else if (selectedType == typeof(ActionInteractableObjectModule))
            {
                ActionInteractableObjectModuleAction.Invoke();
            }

        }
    }
}
