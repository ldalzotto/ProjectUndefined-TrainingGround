using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;
using System.Collections.Generic;
using Editor_PuzzleGameCreationWizard;
using UnityEditor;
using CreationWizard;

namespace Editor_AICreationObjectCreationWizard
{
    [System.Serializable]
    public class AIPrefabCreation : CreateablePrefabComponent<NPCAIManager>
    {
        public override Func<AbstractCreationWizardEditorProfile, NPCAIManager> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
                {
                    return editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleAICommonPrefabs.AIBasePrefab;
                };
            }
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
            base.OnInspectorGUIImpl(serializedObject, editorProfile);
        }
        
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdAI = this.Create(editorInformationsData.CommonGameConfigurations.InstancePath.AIPrefabPaths, editorInformationsData.AiID.ToString() + NameConstants.BaseAIPrefab + "V2");
            createdAI.AiID = editorInformationsData.AiID;
            PrefabUtility.SavePrefabAsset(createdAI.gameObject);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { createdAI });
        }
    }

}
