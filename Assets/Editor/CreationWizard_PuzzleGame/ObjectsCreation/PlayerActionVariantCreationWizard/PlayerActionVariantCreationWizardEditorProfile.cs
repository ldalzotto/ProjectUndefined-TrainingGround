using UnityEngine;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionVariantCreationWizardEditorProfile", menuName = "CreationWizard/PuzzleObjectCreationWizard/PlayerActionVariantCreationWizardEditorProfile", order = 1)]
    public class PlayerActionVariantCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {
   
        public override void OnEnable()
        {
            base.OnEnable();
            this.InitModule<GenericInformations>(false, true, false);
            this.InitModule<GameConfiguration>(false, true, false);
            this.InitModule<AttractiveObjectActionInherentDataCreation>(false, true, false);
            this.InitModule<AddToLevel>(false, false, true);
            this.InitModule<WheelActionCreation>(false, true, false);
        }

        public override void OnGenerationEnd()
        {

        }


    }

}