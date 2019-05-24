using UnityEngine;
using System.Collections;
using RTPuzzle;
using System.Collections.Generic;
using CreationWizard;
using Editor_PuzzleGameCreationWizard;
using System;
using UnityEditor;

namespace Editor_PuzzleLevelCreationWizard
{
    public class LevelCompletionCreation : CreateableScriptableObjectComponent<LevelCompletionInherentData>
    {

        protected override string objectFieldLabel => typeof(LevelCompletionInherentData).Name;

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            var LevelConfigurationCreation = editorProfile.GetModule<LevelConfigurationCreation>();
            return ErrorHelper.ModuleIgnoredIfIsNew(LevelConfigurationCreation.IsNew, nameof(LevelConfigurationCreation), this.GetType().Name);
        }
        
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var LevelConfigurationCreation = editorProfile.GetModule<LevelConfigurationCreation>();
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            if (LevelConfigurationCreation.IsNew)
            {
                var createdCompletionInherentData = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.LevelCompletionDataPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.LevelCompletionInherentData,
                    editorProfile);
                LevelConfigurationCreation.CreatedObject.LevelCompletionInherentData = createdCompletionInherentData;
                     }
        }

    }

}
