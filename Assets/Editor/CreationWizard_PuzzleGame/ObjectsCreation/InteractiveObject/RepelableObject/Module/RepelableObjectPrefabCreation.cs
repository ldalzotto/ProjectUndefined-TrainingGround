using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;
using UnityEditor;

namespace Editor_RepelableObjectCreationWizard
{
    [System.Serializable]
    public class RepelableObjectPrefabCreation : CreateablePrefabComponent<ObjectRepelTypeModule>
    {
        public override Func<AbstractCreationWizardEditorProfile, ObjectRepelTypeModule> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
                    null;
                    //editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseRepelableObjectPrefab;
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.Create(editorInformations.CommonGameConfigurations.InstancePath.RepelableObjectPrefabPath, editorInformations.RepelableObjectID.ToString() + NameConstants.RepelableObjectPrefab, editorProfile);
            this.CreatedPrefab.RepelableObjectID = editorInformations.RepelableObjectID;
            PrefabUtility.SavePrefabAsset(this.CreatedPrefab.gameObject);
        }
    }
}