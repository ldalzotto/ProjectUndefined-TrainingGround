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
        
        public GenericInformation GenericInformation;
        
        public ModelCreation ModelCreation;
        
        public Configurationretrieval ConfigurationRetrieval;
        public AIFeedbackMarkCreation AIFeedbackMarkCreation;

        #region GUIManager
        public AttractiveObjectInherentDataModule AttractiveObjectInherentData;
        #endregion

        public override void OnEnable()
        {
            base.OnEnable();
            if (this.GenericInformation == null)
            {
                this.GenericInformation = GenericInformation.Create<GenericInformation>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(GenericInformation).Name + ".asset");
            }

            if (this.GenericInformation.AttractiveObjectBasePrefab == null)
            {
                this.GenericInformation.AttractiveObjectBasePrefab = AssetFinder.SafeSingleAssetFind<AttractiveObjectType>(AttractiveObjectVariantCreationWizardEditorProfile.GenericAttractiveObjectprefabName);
            }
            if(this.GenericInformation.AIFeedbackMarkBasePrefab == null)
            {
                this.GenericInformation.AIFeedbackMarkBasePrefab = AssetFinder.SafeSingleAssetFind<AIFeedbackMarkType>(AIFeedbackBasePrefabName);
            }

            if (this.ModelCreation == null)
            {
                this.ModelCreation = GenericInformation.Create<ModelCreation>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(ModelCreation).Name + ".asset");
            }
            if (this.ConfigurationRetrieval == null)
            {
                this.ConfigurationRetrieval = GenericInformation.Create<Configurationretrieval>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(Configurationretrieval).Name + ".asset");
            }
            if (this.AttractiveObjectInherentData == null)
            {
                this.AttractiveObjectInherentData = GenericInformation.Create<AttractiveObjectInherentDataModule>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(AttractiveObjectInherentDataModule).Name + ".asset");
            }
            if(this.AIFeedbackMarkCreation == null)
            {
                this.AIFeedbackMarkCreation = GenericInformation.Create<AIFeedbackMarkCreation>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(AIFeedbackMarkCreation).Name + ".asset");
            }
        }

        public override void ResetEditor()
        {
            this.OnEnable();
            base.ResetEditor();
            this.ModelCreation.ResetEditor();
            this.AttractiveObjectInherentData.ResetEditor();
        }

        public override void OnGenerationEnd()
        {
            this.AttractiveObjectInherentData.OnGenerationEnd();
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

