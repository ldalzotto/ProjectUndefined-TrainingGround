//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CreationWizard;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_PuzzleCutsceneCreationWizard
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
            return string.Empty;
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return new List<string>()
            {
              ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.PuzzleCutsceneID, this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.PuzzleCutsceneConfiguration)
            }.Find(s => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        [CustomEnum(isCreateable: true)]
        public PuzzleCutsceneID PuzzleCutsceneID;
        public CommonGameConfigurations CommonGameConfigurations;
    }
}