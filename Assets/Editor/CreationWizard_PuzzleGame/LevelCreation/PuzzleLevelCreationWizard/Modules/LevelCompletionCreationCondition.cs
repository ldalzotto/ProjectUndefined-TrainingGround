using UnityEngine;
using System.Collections;
using RTPuzzle;
using System.Collections.Generic;
using CreationWizard;
using Editor_PuzzleGameCreationWizard;
using System;

namespace Editor_PuzzleLevelCreationWizard
{
    public class LevelCompletionCreationCondition : CreateableScriptableObjectComponent<LevelCompletionConditionConfiguration>
    {
        public LevelCompletionCreationCondition(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => this.GetType().Name;

        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            var modules = PuzzleLevelCreationWizardEditorProfile.GetAllModules(editorModules);
            return ErrorHelper.ModuleIgnoredIfIsNew(modules.LevelCompletionCreation.IsNew, nameof(modules.LevelCompletionCreationCondition), nameof(modules.LevelCompletionCreation));
        }

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, LevelCompletionCreation LevelCompletionCreation, Action<UnityEngine.Object[]> addToGenerated)
        {
            if (LevelCompletionCreation.IsNew)
            {
                var createdCompletionInherentData = this.CreateAsset(editorInformationsData.LevelCompletionConfigurationDataPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.LevelCompletionConditionConfigurationName);
                LevelCompletionCreation.CreatedObject.LevelCompletionConditionConfiguration = createdCompletionInherentData;
                addToGenerated.Invoke(new UnityEngine.Object[] { createdCompletionInherentData });
            }
        }
    }
}

