using CreationWizard;
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
                    ObjectRepelTypeModuleAction: () => { returnDescription = "Allow the object to be repelled by projectile."; }
                );
            return returnDescription;
        }

        private void InteractiveObjectModuleSwitch(Type selectedType, Action InteractiveObjectModuleAction, Action ObjectRepelTypeModuleAction)
        {
            if (selectedType == typeof(ModelObjectModule))
            {
                InteractiveObjectModuleAction.Invoke();
            }
            else if (selectedType == typeof(ObjectRepelTypeModule))
            {
                ObjectRepelTypeModuleAction.Invoke();
            }
        }
    }
}
