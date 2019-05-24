using UnityEngine;
using System.Collections;
using UnityEditor;
using Editor_PuzzleGameCreationWizard;
using System.Collections.Generic;
using CreationWizard;
using RTPuzzle;
using System.Linq;
using System;

namespace Editor_TargetZoneCreationWizard
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
                ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.TargetZoneID, this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.TargetZonesConfiguration)
            }
            .Find(s => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        public TargetZoneID TargetZoneID;
        public CommonGameConfigurations CommonGameConfigurations;
    }
}