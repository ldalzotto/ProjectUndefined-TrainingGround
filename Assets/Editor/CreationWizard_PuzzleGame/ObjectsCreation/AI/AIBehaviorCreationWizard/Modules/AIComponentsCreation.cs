using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using Editor_MainGameCreationWizard;

namespace Editor_AIBehaviorCreationWizard
{
    [System.Serializable]
    public class AIComponentsCreation : CreateableScriptableObjectComponent<AbstractAIComponents>
    {

        private List<Type> AIComponentsType;

        [SerializeField]
        private int selectedType;

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
            EditorGUILayout.ObjectField(this.CreatedObject, typeof(AbstractAIComponents), true);
            if (this.AIComponentsType == null)
            {
                this.AIComponentsType = TypeHelper.GetAllTypeAssignableFrom(typeof(AbstractAIComponents)).ToList();
            }

            if (this.AIComponentsType != null)
            {
                EditorGUILayout.LabelField("Select the behavior you want to create : ");
                EditorGUI.BeginChangeCheck();
                this.selectedType = EditorGUILayout.Popup(this.selectedType, this.AIComponentsType.ConvertAll(t => t.Name).ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    this.InstanciateInEditor(editorProfile);
                }
                if (this.CreatedObject == null)
                {
                    this.InstanciateInEditor(editorProfile);
                }
            }
        }

        public override void InstanciateInEditor(AbstractCreationWizardEditorProfile editorProfile)
        {
            if (this.AIComponentsType == null)
            {
                this.AIComponentsType = TypeHelper.GetAllTypeAssignableFrom(typeof(AbstractAIComponents)).ToList();
            }
            this.CreatedObject = (AbstractAIComponents)ScriptableObject.CreateInstance(this.AIComponentsType[this.selectedType].Name);
            this.isNew = true;
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdAIComponents = this.CreateAsset(InstancePath.AIBehaviorConfigurationPath, editorInformations.AiID.ToString() + NameConstants.AIComponents, editorProfile);
            this.CreatedObject = createdAIComponents;

            var aiBehavrioConfiguration = editorInformations.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration.ConfigurationInherentData[editorInformations.AiID];
            var aiBehavrioConfigurationSerialized = new SerializedObject(aiBehavrioConfiguration);
            aiBehavrioConfigurationSerialized.FindProperty(nameof(aiBehavrioConfiguration.AIComponents)).objectReferenceValue= createdAIComponents;
            aiBehavrioConfigurationSerialized.ApplyModifiedProperties();

            var componentsFolderPath = InstancePath.AIBehaviorConfigurationPath + "/" + editorInformations.AiID.ToString() + "_Components";
            this.CreateFolderIfNecessary(componentsFolderPath);

            var createdAIComponentsSerialied = new SerializedObject(createdAIComponents);

            List<UnityEngine.Object> generatedComponents = new List<UnityEngine.Object>();
            foreach (var aiComponentField in createdAIComponents.GetType().GetFields())
            {
                if (aiComponentField.FieldType.IsSubclassOf(typeof(AbstractAIComponent)))
                {
                    var so = ScriptableObject.CreateInstance(aiComponentField.FieldType.Name);
                    var generatedComponent = new GeneratedScriptableObjectManager<AbstractAIComponent>((AbstractAIComponent)so,
                          componentsFolderPath, editorInformations.AiID.ToString() + "_" + aiComponentField.Name);

                    createdAIComponentsSerialied.FindProperty(aiComponentField.Name).objectReferenceValue = generatedComponent.GeneratedAsset;
                    generatedComponents.Add(generatedComponent.GeneratedAsset);
                }
            }

            createdAIComponentsSerialied.ApplyModifiedProperties();


            if (generatedComponents.Count > 0)
            {
                editorProfile.AddToGeneratedObjects(generatedComponents.ToArray());
            }
        }
    }
}