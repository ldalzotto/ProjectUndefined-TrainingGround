using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;
using UnityEditor;
using UnityEngine;

namespace Editor_AttractiveObjectCreationWizard
{
    [System.Serializable]
    public class AttractiveObjectPrefabCreation : CreateablePrefabComponent<InteractiveObjectType>
    {
        public override Func<AbstractCreationWizardEditorProfile, InteractiveObjectType> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
                {
                    //TODO
                    return null;// editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseAttractiveObjectPrefab;
                };
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.Create(editorInformationsData.CommonGameConfigurations.InstancePath.AttractiveObjectPrefabPath, editorInformationsData.AttractiveObjectId.ToString() + NameConstants.AttractiveObjectPrefab, editorProfile);
            this.CreatedPrefab.GetComponentInChildren<AttractiveObjectTypeModule>().AttractiveObjectId = editorInformationsData.AttractiveObjectId;
            PrefabUtility.SavePrefabAsset(this.CreatedPrefab.gameObject);
        }
    }
}