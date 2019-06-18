using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using Editor_MainGameCreationWizard;
using System.Collections.Generic;
using CreationWizard;
using System.Linq;
using System;
using GameConfigurationID;

namespace Editor_AIBehaviorCreationWizard
{
    [System.Serializable]
    public class EditorInformations : CreationModuleComponent
    {

        public EditorInformationsData EditorInformationsData;

        public override void ResetEditor()
        {

        }

        private void InitProperties()
        {
            EditorInformationsHelper.InitProperties(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.EditorInformationsData)), true);
        }

        public override string ComputeErrorState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return new List<string>() {
                EditorInformationsHelper.ComputeErrorState(ref this.EditorInformationsData.CommonGameConfigurations) ,
                ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AiID, this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                nameof(this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration.ConfigurationInherentData))
            }.Find((s) => !string.IsNullOrEmpty(s));
        }

    }


    [System.Serializable]
    public class EditorInformationsData
    {
        public AiID AiID;
        public CommonGameConfigurations CommonGameConfigurations;
    }
}