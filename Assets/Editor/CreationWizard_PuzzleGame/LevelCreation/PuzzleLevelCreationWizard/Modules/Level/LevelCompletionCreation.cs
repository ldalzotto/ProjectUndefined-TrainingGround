using UnityEngine;
using System.Collections;
using RTPuzzle;
using System.Collections.Generic;
using CreationWizard;
using Editor_PuzzleGameCreationWizard;
using System;

namespace Editor_PuzzleLevelCreationWizard
{
    public class LevelCompletionCreation : CreateableScriptableObjectComponent<LevelCompletionInherentData>
    {
        public LevelCompletionCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => typeof(LevelCompletionInherentData).Name;

        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            var modules = PuzzleLevelCreationWizardEditorProfile.GetAllModules(editorModules);
            return ErrorHelper.ModuleIgnoredIfIsNew(modules.LevelConfigurationCreation.IsNew, nameof(modules.LevelConfigurationCreation), this.GetType().Name);
        }

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, LevelConfigurationCreation LevelConfigurationCreation, Action<UnityEngine.Object[]> addToGenerated)
        {
            if (LevelConfigurationCreation.IsNew)
            {
                var createdCompletionInherentData = this.CreateAsset(editorInformationsData.InstancePath.LevelCompletionDataPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.LevelCompletionInherentData);
                LevelConfigurationCreation.CreatedObject.LevelCompletionInherentData = createdCompletionInherentData;
                addToGenerated.Invoke(new UnityEngine.Object[] { createdCompletionInherentData });
            }
        }
    }

}
