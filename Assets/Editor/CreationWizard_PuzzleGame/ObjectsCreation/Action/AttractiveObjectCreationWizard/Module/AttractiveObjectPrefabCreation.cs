using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;

namespace Editor_AttractiveObjectCreationWizard
{
    [System.Serializable]
    public class AttractiveObjectPrefabCreation : CreateablePrefabComponent<AttractiveObjectType>
    {
        public override Func<AbstractCreationWizardEditorProfile, AttractiveObjectType> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
                {
                    return editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseAttractiveObjectPrefab;
                };
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.Create(editorInformationsData.CommonGameConfigurations.InstancePath.AttractiveObjectPrefabPath, editorInformationsData.AttractiveObjectId.ToString() + NameConstants.AttractiveObjectPrefab, editorProfile);
            this.CreatedPrefab.AttractiveObjectId = editorInformationsData.AttractiveObjectId;
        }
    }
}