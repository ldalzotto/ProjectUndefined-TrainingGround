using ConfigurationEditor;
using CreationWizard;
using Editor_MainGameCreationWizard;
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

        public bool DeepDeletion { get => deepDeletion; set => deepDeletion = value; }


        protected override List<InteractiveObjectModule> GetModules(InteractiveObjectType RootModuleObject)
        {
            return RootModuleObject.GetComponentsInChildren<InteractiveObjectModule>().ToList();
        }

        protected override void OnEdit(InteractiveObjectType InteractiveObjectType, Type selectedType)
        {
            if (InteractiveObjectModuleWizardConfiguration.InteractiveObjectModuleConfiguration.ContainsKey(selectedType))
            {
                var editOperation = InteractiveObjectModuleWizardConfiguration.InteractiveObjectModuleConfiguration[selectedType].InteractiveObjectModuleEditOperation;
                if (editOperation.GetType() == typeof(InteractiveObjectNonIdentifiedEditOperation))
                {
                    ((InteractiveObjectNonIdentifiedEditOperation)editOperation).ProcessEdit(InteractiveObjectType, this, this.CommonGameConfigurations);
                }
                else
                {
                    ((InteractiveObjectIdentifiedEditOperation)editOperation).ProcessEdit(InteractiveObjectType, this, this.CommonGameConfigurations, this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID);
                }
            }
        }
        
        protected override bool AdditionalEditCondition(Type selectedType)
        {
            var PrefabEditContidionOperation = InteractiveObjectModuleWizardConfiguration.InteractiveObjectModuleConfiguration[selectedType].PrefabEditContidionOperation;
            if (PrefabEditContidionOperation.GetType() == typeof(NoPrefabEditConditionOperation))
            {
                return ((NoPrefabEditConditionOperation)PrefabEditContidionOperation).AdditionalEditCondition();
            }
            else
            {
                return ((PrefabIdentifiableConditionOperation)PrefabEditContidionOperation).AdditionalEditCondition(this, this.CommonGameConfigurations, this.GameDesignerEditorProfile.InteractiveObjectModuleWizardID, this.GameDesignerEditorProfileSO);
            }
        }

        protected override string POIModuleDescription(Type selectedType)
        {
            if (InteractiveObjectModuleWizardConfiguration.InteractiveObjectModuleConfiguration.ContainsKey(selectedType))
            {
                return InteractiveObjectModuleWizardConfiguration.InteractiveObjectModuleConfiguration[selectedType].Description;
            }
            return string.Empty;
        }

    }

    public class InteractiveObjectModuleConfigurationProfile
    {
        public InteractiveObjectModuleEditOperation InteractiveObjectModuleEditOperation;
        public PrefabEditContidionOperation PrefabEditContidionOperation;
        public string Description;

        public InteractiveObjectModuleConfigurationProfile(InteractiveObjectModuleEditOperation interactiveObjectModuleEditOperation, PrefabEditContidionOperation prefabEditContidionOperation, string description)
        {
            InteractiveObjectModuleEditOperation = interactiveObjectModuleEditOperation;
            PrefabEditContidionOperation = prefabEditContidionOperation;
            Description = description;
        }
    }

    public abstract class InteractiveObjectModuleEditOperation { }

    public class InteractiveObjectIdentifiedEditOperation : InteractiveObjectModuleEditOperation
    {
        private Func<CommonGameConfigurations, InteractiveObjectModule> interactiveObjectModulePrefab;
        private Func<CommonGameConfigurations, IConfigurationSerialization> configuration;
        private Func<InteractiveObjectModule, Enum> GetModuleID;
        private Action<InteractiveObjectModule, Enum> SetModuleID;
        private Func<InteractiveObjectModuleWizardID, Enum> moduleIDToSet;

        public InteractiveObjectIdentifiedEditOperation(Func<CommonGameConfigurations, InteractiveObjectModule> interactiveObjectModulePrefab, Func<CommonGameConfigurations, IConfigurationSerialization> configuration,
                    Func<InteractiveObjectModule, Enum> getModuleID, Action<InteractiveObjectModule, Enum> setModuleID, Func<InteractiveObjectModuleWizardID, Enum> moduleIDToSet)
        {
            this.interactiveObjectModulePrefab = interactiveObjectModulePrefab;
            this.configuration = configuration;
            GetModuleID = getModuleID;
            SetModuleID = setModuleID;
            this.moduleIDToSet = moduleIDToSet;
        }

        public void ProcessEdit(InteractiveObjectType interactiveObjectType, InteractiveObjectModuleWizard InteractiveObjectModuleWizard, CommonGameConfigurations CommonGameConfigurations, InteractiveObjectModuleWizardID InteractiveObjectModuleWizardID)
        {
            if (InteractiveObjectModuleWizard.Add)
            {
                var objectRepelTypeModule = EditorInteractiveObjectModulesOperation.AddPrefabModule(interactiveObjectType, interactiveObjectModulePrefab.Invoke(CommonGameConfigurations));
                SetModuleID.Invoke(objectRepelTypeModule, moduleIDToSet.Invoke(InteractiveObjectModuleWizardID));
                EditorUtility.SetDirty(InteractiveObjectModuleWizard.CurrentSelectedObjet);
            }
            else if (InteractiveObjectModuleWizard.Remove)
            {
                if (InteractiveObjectModuleWizard.DeepDeletion)
                {
                    //Get ID
                    var RetrievedObjectRepelTypeModule = (InteractiveObjectModule)interactiveObjectType.GetComponentInChildren(interactiveObjectModulePrefab.Invoke(CommonGameConfigurations).GetType());
                    if (RetrievedObjectRepelTypeModule != null)
                    {
                        var ojectRepelID = GetModuleID.Invoke(RetrievedObjectRepelTypeModule);

                        //configuration deletion
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(configuration.Invoke(CommonGameConfigurations).GetEntry(ojectRepelID)));
                        configuration.Invoke(CommonGameConfigurations).ClearEntry(ojectRepelID);
                    }
                }
                EditorInteractiveObjectModulesOperation.RemovePrefabModule(interactiveObjectType, interactiveObjectModulePrefab.Invoke(CommonGameConfigurations).GetType());
            }
        }
    }

    public class InteractiveObjectNonIdentifiedEditOperation : InteractiveObjectModuleEditOperation
    {
        private Func<CommonGameConfigurations, InteractiveObjectModule> interactiveObjectModulePrefab;

        public InteractiveObjectNonIdentifiedEditOperation(Func<CommonGameConfigurations, InteractiveObjectModule> interactiveObjectModulePrefab)
        {
            this.interactiveObjectModulePrefab = interactiveObjectModulePrefab;
        }

        public void ProcessEdit(InteractiveObjectType interactiveObjectType, InteractiveObjectModuleWizard InteractiveObjectModuleWizard, CommonGameConfigurations CommonGameConfigurations)
        {
            if (InteractiveObjectModuleWizard.Add)
            {
                EditorInteractiveObjectModulesOperation.AddPrefabModule(interactiveObjectType, interactiveObjectModulePrefab.Invoke(CommonGameConfigurations));
            }
            else if (InteractiveObjectModuleWizard.Remove)
            {
                EditorInteractiveObjectModulesOperation.RemovePrefabModule(interactiveObjectType, interactiveObjectModulePrefab.Invoke(CommonGameConfigurations).GetType());
            }
        }
    }

    public abstract class PrefabEditContidionOperation { }
    public class NoPrefabEditConditionOperation : PrefabEditContidionOperation
    {
        public bool AdditionalEditCondition()
        {
            return true;
        }
    }

    public class PrefabIdentifiableConditionOperation : PrefabEditContidionOperation
    {
        private Func<CommonGameConfigurations, IConfigurationSerialization> configuration;
        private Func<InteractiveObjectModuleWizardID, Enum> moduleIDToSet;
        private Type creationWizardType;

        public PrefabIdentifiableConditionOperation(Func<CommonGameConfigurations, IConfigurationSerialization> configuration, Func<InteractiveObjectModuleWizardID, Enum> moduleIDToSet, Type creationWizardType)
        {
            this.configuration = configuration;
            this.moduleIDToSet = moduleIDToSet;
            this.creationWizardType = creationWizardType;
        }

        public bool AdditionalEditCondition(InteractiveObjectModuleWizard interactiveObjectModuleWizard, CommonGameConfigurations commonGameConfigurations,
                                    InteractiveObjectModuleWizardID InteractiveObjectModuleWizardID, SerializedObject GameDesignerEditorProfileSO)
        {
            bool allowedToEdit = false;
            var idSerializedProperty = GameDesignerEditorProfileSO.FindProperty("InteractiveObjectModuleWizardID." + this.moduleIDToSet.Invoke(InteractiveObjectModuleWizardID).GetType().Name);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(idSerializedProperty);
            if (GUILayout.Button("CREATE ID BASED ON OBJECT NAME"))
            {
                string preFilledName = interactiveObjectModuleWizard.CurrentSelectedObjet.name.Substring(0, interactiveObjectModuleWizard.CurrentSelectedObjet.name.LastIndexOf("_"));
                EnumIDGeneration.Init(moduleIDToSet.Invoke(InteractiveObjectModuleWizardID).GetType(), preFilledName);
            }
            string isError = ErrorHelper.NotAlreadyPresentInConfiguration(moduleIDToSet.Invoke(InteractiveObjectModuleWizardID), configuration.Invoke(commonGameConfigurations).GetKeys(), nameof(configuration));
            if (!string.IsNullOrEmpty(isError))
            {
                EditorGUILayout.HelpBox(isError, MessageType.Error);
                if (GUILayout.Button("CREATE IN EDITOR"))
                {
                    GameCreationWizard.InitWithSelected(this.creationWizardType.Name);
                }
            }
            else
            {
                allowedToEdit = true;
            }

            if (interactiveObjectModuleWizard.Remove)
            {
                interactiveObjectModuleWizard.DeepDeletion = GUILayout.Toggle(interactiveObjectModuleWizard.DeepDeletion, "Deep deletion", EditorStyles.miniButton, GUILayout.Width(150f));
            }

            EditorGUILayout.Separator();

            return allowedToEdit;
        }
    }
}
