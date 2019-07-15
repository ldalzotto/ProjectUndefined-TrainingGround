﻿using UnityEngine;
using System.Collections;
using AdventureGame;
using Editor_MainGameCreationWizard;
using UnityEditor;
using System.Collections.Generic;
using CreationWizard;
using GameConfigurationID;

namespace Editor_AnimationCreationWizard
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
                ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.AnimationID, this.EditorInformationsData.CommonGameConfigurations.CoreGameConfigurations.AnimationConfiguration)
            }
            .Find(s => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        [CustomEnum(isCreateable: true)]
        public AnimationID AnimationID;
        public CommonGameConfigurations CommonGameConfigurations;
    }
}