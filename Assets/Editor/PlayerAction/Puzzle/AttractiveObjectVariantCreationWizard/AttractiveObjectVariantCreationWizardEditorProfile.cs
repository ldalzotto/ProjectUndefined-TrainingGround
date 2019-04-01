using RTPuzzle;
using UnityEngine;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectVariantCreationWizardEditorProfile", menuName = "PlayerAction/Puzzle/AttractiveObject/AttractiveObjectVariantCreationWizardEditorProfile", order = 1)]
    public class AttractiveObjectVariantCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {
        public const string GenericAttractiveObjectprefabName = "GenericAttractiveObjectPrefab";
        public const string AIFeedbackBasePrefabName = "GenericAIMark";

        /*
        public GenericInformation GenericInformation;
        
        public ModelCreation ModelCreation;
        
        public Configurationretrieval ConfigurationRetrieval;
        public AIFeedbackMarkCreation AIFeedbackMarkCreation;

        #region GUIManager
        public AttractiveObjectInherentDataModule AttractiveObjectInherentData;
        #endregion
        */
        public override void OnEnable()
        {
            base.OnEnable();
            this.InitModule<GenericInformation>(false, true, false);
            this.InitModule<ModelCreation>(false, true, false);
            this.InitModule<Configurationretrieval>(false, true, false);
            this.InitModule<AttractiveObjectInherentDataModule>(false, true, false);
            this.InitModule<AIFeedbackMarkCreation>(false, true, false);

            var genericInformation = this.GetModule<GenericInformation>();

            if (genericInformation.AttractiveObjectBasePrefab == null)
            {
                genericInformation.AttractiveObjectBasePrefab = AssetFinder.SafeSingleAssetFind<AttractiveObjectType>(AttractiveObjectVariantCreationWizardEditorProfile.GenericAttractiveObjectprefabName);
            }
            if (genericInformation.AIFeedbackMarkBasePrefab == null)
            {
                genericInformation.AIFeedbackMarkBasePrefab = AssetFinder.SafeSingleAssetFind<AIFeedbackMarkType>(AIFeedbackBasePrefabName);
            }

        }

        public override void ResetEditor()
        {
            var modelCreation = this.GetModule<ModelCreation>();
            var attractiveObjectInherentData = this.GetModule<AttractiveObjectInherentDataModule>();

            this.OnEnable();
            base.ResetEditor();
            modelCreation.ResetEditor();
            attractiveObjectInherentData.ResetEditor();
        }

        public override void OnGenerationEnd()
        {
            var attractiveObjectInherentData = this.GetModule<AttractiveObjectInherentDataModule>();
            attractiveObjectInherentData.OnGenerationEnd();
        }
    }

    [System.Serializable]
    public class PathConfiguration
    {
        public string ObjectPrefabFolder;
        public string ObjectConfigurationFolder;
        public string AIMarkPrefabFolder;
    }

}

