using CreationWizard;
using Editor_AttractiveObjectCreationWizard;
using Editor_MainGameCreationWizard;
using Editor_RepelableObjectCreationWizard;
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
               }
             );
        }

        protected override bool AdditionalEditCondition(Type selectedType)
        {
            bool allowedToEdit = false;
            this.InteractiveObjectModuleSwitch(selectedType,
                    InteractiveObjectModuleAction: () => allowedToEdit = true,
                    ObjectRepelTypeModuleAction: () =>
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.PropertyField(this.GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID.RepelableObjectID"));
                        if (GUILayout.Button("CREATE ID BASED ON OBJECT NAME"))
                        {
                            string preFilledName = this.currentSelectedObjet.name.Substring(0, this.currentSelectedObjet.name.LastIndexOf("_"));
                            EnumIDGeneration.Init(this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.RepelableObjectID.GetType(), preFilledName);
                        }
                        string isError = ErrorHelper.NotAlreadyPresentInConfiguration(this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.RepelableObjectID, this.CommonGameConfigurations.PuzzleGameConfigurations.RepelableObjectsConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e), nameof(this.CommonGameConfigurations.PuzzleGameConfigurations.RepelableObjectsConfiguration));
                        if (!string.IsNullOrEmpty(isError))
                        {
                            EditorGUILayout.HelpBox(isError, MessageType.Error);
                            if (GUILayout.Button("CREATE IN EDITOR"))
                            {
                                GameCreationWizard.InitWithSelected(typeof(RepelableObjectCreationWizard).Name);
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
                    },
                    AttractiveObjectTypeAction: () =>
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.PropertyField(this.GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID.AttractiveObjectId"));
                        if (GUILayout.Button("CREATE ID BASED ON OBJECT NAME"))
                        {
                            string preFilledName = this.currentSelectedObjet.name.Substring(0, this.currentSelectedObjet.name.LastIndexOf("_"));
                            EnumIDGeneration.Init(this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.AttractiveObjectId.GetType(), preFilledName);
                        }
                        string isError = ErrorHelper.NotAlreadyPresentInConfiguration(this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID.AttractiveObjectId, this.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e), nameof(this.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration));
                        if (!string.IsNullOrEmpty(isError))
                        {
                            EditorGUILayout.HelpBox(isError, MessageType.Error);
                            if (GUILayout.Button("CREATE IN EDITOR"))
                            {
                                GameCreationWizard.InitWithSelected(typeof(AttractiveObjectCreationWizard).Name);
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
                );
            return allowedToEdit;
        }

        protected override string POIModuleDescription(Type selectedType)
        {
            string returnDescription = string.Empty;
            this.InteractiveObjectModuleSwitch(selectedType,
                    InteractiveObjectModuleAction: () => { returnDescription = "Model object definition."; },
                    ObjectRepelTypeModuleAction: () => { returnDescription = "Allow the object to be repelled by projectile."; },
                    AttractiveObjectTypeAction: () => { returnDescription = "Attract AI when on range."; }
                );
            return returnDescription;
        }

        private void InteractiveObjectModuleSwitch(Type selectedType, Action InteractiveObjectModuleAction, Action ObjectRepelTypeModuleAction, Action AttractiveObjectTypeAction)
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
        }
    }
}
