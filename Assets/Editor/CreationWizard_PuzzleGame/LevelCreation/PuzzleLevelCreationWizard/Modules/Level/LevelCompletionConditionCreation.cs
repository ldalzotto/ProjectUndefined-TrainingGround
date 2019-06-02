using UnityEngine;
using System.Collections;
using RTPuzzle;
using System.Collections.Generic;
using CreationWizard;
using Editor_MainGameCreationWizard;
using System;

namespace Editor_PuzzleLevelCreationWizard
{
    public class LevelCompletionConditionCreation : CreateableScriptableObjectComponent<ConditionGraphEditorProfile>
    {
        protected override string objectFieldLabel => this.GetType().Name;

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            var LevelCompletionCreation = editorProfile.GetModule<LevelCompletionCreation>();
            var LevelCompletionCreationCondition = editorProfile.GetModule<LevelCompletionConditionCreation>();
            return ErrorHelper.ModuleIgnoredIfIsNew(LevelCompletionCreation.IsNew, nameof(LevelCompletionCreationCondition), nameof(LevelCompletionCreation));
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var LevelCompletionCreation = editorProfile.GetModule<LevelCompletionCreation>();
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            if (LevelCompletionCreation.IsNew)
            {
                var createdCompletionInherentData = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.LevelCompletionConditionDataPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.LevelCompletionConditionConfigurationName, editorProfile);
                LevelCompletionCreation.CreatedObject.ConditionGraphEditorProfile = createdCompletionInherentData;
              }
        }
    }
}

