using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;
using Editor_MainGameCreationWizard;
using UnityEditor;

namespace Editor_TargetZoneCreationWizard
{
    [System.Serializable]
    public class TargetZonePrefabCreation : CreateablePrefabComponent<TargetZoneObjectModule>
    {
        public override Func<AbstractCreationWizardEditorProfile, TargetZoneObjectModule> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
                 {
                     return editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseTargetZonePrefab;
                 };
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformation = editorProfile.GetModule<EditorInformations>();
            var createdTargetZone = this.Create(editorInformation.EditorInformationsData.CommonGameConfigurations.InstancePath.TargetZonePrefabPath, editorInformation.EditorInformationsData.TargetZoneID.ToString() + NameConstants.TargetZonePrefab, editorProfile);
            createdTargetZone.TargetZoneID = editorInformation.EditorInformationsData.TargetZoneID;
            PrefabUtility.SavePrefabAsset(createdTargetZone.gameObject);
        }
    }
}