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
        public GameObject AIModel;
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.AIModel)));
            base.OnInspectorGUIImpl(serializedObject, editorProfile);
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            return ErrorHelper.NonNullity(this.AIModel, nameof(this.AIModel));
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdAI = this.Create(editorInformationsData.CommonGameConfigurations.InstancePath.AIPrefabPaths, editorInformationsData.AiID.ToString() + NameConstants.BaseAIPrefab + "V2");
            this.CreatedPrefab.AiID = editorInformationsData.AiID;
           
            PrefabUtility.InstantiatePrefab(this.AIModel, this.CreatedPrefab.gameObject.FindChildObjectRecursively("Model").transform);
            PrefabUtility.SavePrefabAsset(createdAI.gameObject);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { createdAI });
        }
    }

}
