using AdventureGame;
using Editor_MainGameCreationWizard;
using System;
using UnityEditor;
using UnityEngine;

namespace Editor_POICreationWizard
{
    [System.Serializable]
    public class POIPrefabCreation : CreateablePrefabComponent<GameObject>
    {
        public override Func<AbstractCreationWizardEditorProfile, GameObject> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
               {
                   return editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.AdventureCommonPrefabs.BasePOIPrefab;
               };
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdPOI = this.Create(InstancePath.POIPrefabPath, editorInformationsData.PointOfInterestID.ToString() + NameConstants.POIPrefab, editorProfile);
            createdPOI.GetComponentInChildren<PointOfInterestType>().PointOfInterestId = editorInformationsData.PointOfInterestID;
            var savedPrefab = PrefabUtility.SavePrefabAsset(createdPOI.gameObject);

            var pointOfInterestInherentData = editorProfile.GetModule<POIConfigurationCreation>().CreatedObject;
            SerializableObjectHelper.Modify(pointOfInterestInherentData, (so) => so.FindProperty(nameof(pointOfInterestInherentData.PointOfInterestPrefab)).objectReferenceValue = savedPrefab);
        }
    }
}