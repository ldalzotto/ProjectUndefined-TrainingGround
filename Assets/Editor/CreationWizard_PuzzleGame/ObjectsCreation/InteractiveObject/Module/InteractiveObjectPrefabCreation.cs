using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;
using UnityEditor;

namespace Editor_InteractiveObjectCreationWizard
{
    [System.Serializable]
    public class InteractiveObjectPrefabCreation : CreateablePrefabComponent<InteractiveObjectType>
    {
        public override Func<AbstractCreationWizardEditorProfile, InteractiveObjectType> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
                        editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseInteractiveObjectTypePrefab;
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var interactiveObject = this.Create(InstancePath.InteractiveObjectPrefabPath, editorInformations.InteractiveObjectID.ToString() + NameConstants.InteractiveObject + editorInformations.ObjectDominantPrefix,
                    editorProfile);
            interactiveObject.InteractiveObjectID = editorInformations.InteractiveObjectID;
            PrefabUtility.SavePrefabAsset(this.CreatedPrefab.gameObject);
        }
    }
}