using CreationWizard;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEditor;

namespace Editor_InteractiveObjectCreationWizard
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
                ErrorHelper.NonNullity(this.EditorInformationsData.ObjectDominantPrefix, nameof(this.EditorInformationsData.ObjectDominantPrefix))
            }.Find(s => !string.IsNullOrEmpty(s));
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return string.Empty;
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        [CustomEnum()]
        public InteractiveObjectID InteractiveObjectID;
        public string ObjectDominantPrefix;
        public CommonGameConfigurations CommonGameConfigurations;
    }

}