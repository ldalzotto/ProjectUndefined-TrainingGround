using CreationWizard;
using Editor_MainGameCreationWizard;
using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_AttractiveObjectCreationWizard
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
                 EditorInformationsHelper.ComputeErrorState(ref this.EditorInformationsData.CommonGameConfigurations),
                 this.AttractiveObjectModelVerification()
            }.Find((s) => !string.IsNullOrEmpty(s));
        }

        private string AttractiveObjectModelVerification()
        {
            if (this.EditorInformationsData.AttractiveObjectModel == null)
            {
                return "AttractiveObjectModel must not be null.";
            }
            return string.Empty;
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return new List<string>()
            {
              ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.AttractiveObjectId, this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration)
            }.Find(s => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        public AttractiveObjectId AttractiveObjectId;
        public GameObject AttractiveObjectModel;
        public CommonGameConfigurations CommonGameConfigurations;
    }
}