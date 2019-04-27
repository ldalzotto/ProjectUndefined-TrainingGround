using UnityEngine;
using System.Collections;

namespace Editor_AIContextMarkGameConfigWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIContextMarkGameConfigEditorProfile", menuName = "PlayerAction/Puzzle/AIVisualFeedback/AIContextMarkGameConfigEditorProfile", order = 1)]
    public class AIContextMarkGameConfigEditorProfile : AbstractCreationWizardEditorProfile
    {
        public override void OnEnable()
        {
            base.OnEnable();
            this.InitModule<GenericInformations>(true, true, false);
            this.InitModule<GameConfiguration>(true, true, false);
            this.InitModule<AIContextMarkConfigurationCreator>(true, true, false);
        }

        public override void OnGenerationEnd()
        {
           
        }
    }

}
