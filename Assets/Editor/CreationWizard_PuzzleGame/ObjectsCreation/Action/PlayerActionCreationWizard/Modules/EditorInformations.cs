using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Editor_MainGameCreationWizard;
using UnityEditor;
using RTPuzzle;
using CreationWizard;
using GameConfigurationID;

namespace Editor_PlayerActionCreationWizard
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
            return EditorInformationsHelper.ComputeErrorState(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return new List<string>()
            {
                ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.PlayerActionId, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<PlayerActionConfiguration>())
            }.Find(s => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        [CustomEnum()]
        public PlayerActionId PlayerActionId;
        [CustomEnum()]
        public LevelZonesID LevelZonesID;
        public CommonGameConfigurations CommonGameConfigurations;
    }
}