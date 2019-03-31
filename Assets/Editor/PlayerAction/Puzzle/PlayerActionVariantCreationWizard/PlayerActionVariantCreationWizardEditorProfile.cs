using UnityEngine;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionVariantCreationWizardEditorProfile", menuName = "PlayerAction/Puzzle/PlayerActionVariantCreationWizardEditorProfile", order = 1)]
    public class PlayerActionVariantCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {
        public GenericInformations GenericInformations;
        public GameConfiguration GameConfiguration;
        public AttractiveObjectActionInherentDataCreation AttractiveObjectActionInherentDataCreation;

        public override void OnEnable()
        {
            base.OnEnable();
            if (this.GenericInformations == null)
            {
                this.GenericInformations = GenericInformations.Create<GenericInformations>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(GenericInformations).Name + ".asset");
            }
            if (this.GameConfiguration == null)
            {
                this.GameConfiguration = GenericInformations.Create<GameConfiguration>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(GameConfiguration).Name + ".asset");
            }
            if (this.AttractiveObjectActionInherentDataCreation == null)
            {
                this.AttractiveObjectActionInherentDataCreation = GenericInformations.Create<AttractiveObjectActionInherentDataCreation>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(AttractiveObjectActionInherentDataCreation).Name + ".asset");
            }
        }

        public override void OnGenerationEnd()
        {

        }


    }

}