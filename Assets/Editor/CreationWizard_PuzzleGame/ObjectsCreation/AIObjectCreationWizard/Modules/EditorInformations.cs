using CreationWizard;
using Editor_PuzzleGameCreationWizard;
using RTPuzzle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_AICreationObjectCreationWizard
{
    [System.Serializable]
    public class EditorInformations : CreationModuleComponent
    {
        public EditorInformationsData EditorInformationsData;

        public EditorInformations(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override void ResetEditor()
        {
        }

        private void InitProperties()
        {
            EditorInformationsHelper.InitProperties(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.EditorInformationsData)), true);
        }

        public override string ComputeErrorState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            return EditorInformationsHelper.ComputeErrorState(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            return new List<string>()
            {
              ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AiID, 
              this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
              this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration.GetType().Name)
            }
            .Find(s => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        public AiID AiID;
        public CommonGameConfigurations CommonGameConfigurations;
    }

}
