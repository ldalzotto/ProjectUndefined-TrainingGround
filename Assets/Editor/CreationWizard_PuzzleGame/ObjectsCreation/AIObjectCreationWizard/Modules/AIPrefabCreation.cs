using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;
using System.Collections.Generic;
using Editor_PuzzleGameCreationWizard;
using UnityEditor;

namespace Editor_AICreationObjectCreationWizard
{
    [System.Serializable]
    public class AIPrefabCreation : CreateablePrefabComponent<NPCAIManager>
    {
        public AIPrefabCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override Func<Dictionary<string, CreationModuleComponent>, NPCAIManager> BasePrefabProvider
        {
            get
            {
                return (Dictionary<string, CreationModuleComponent> imodules) =>
                {
                    var modules = AIObjectCreationWizardProfile.GetAllModules(imodules);
                    return modules.EditorInformations.EditorInformationsData.CommonGameConfigurations.PuzzleAICommonPrefabs.AIBasePrefab;
                };
            }
        }

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, AbstractCreationWizardEditorProfile editorProfile)
        {
            var createdAI = this.Create(editorInformationsData.CommonGameConfigurations.InstancePath.AIPrefabPaths, editorInformationsData.AiID.ToString() + NameConstants.BaseAIPrefab);
            createdAI.AiID = editorInformationsData.AiID;
            PrefabUtility.SavePrefabAsset(createdAI.gameObject);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { createdAI });
        }
    }

}
