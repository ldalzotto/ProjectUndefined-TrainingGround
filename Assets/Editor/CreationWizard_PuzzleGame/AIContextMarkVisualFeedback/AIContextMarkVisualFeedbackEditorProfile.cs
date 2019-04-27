using UnityEngine;
using System.Collections;

namespace Editor_AIContextMarkVisualFeedbackCreationWizardEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIContextMarkVisualFeedbackEditorProfile", menuName = "PlayerAction/Puzzle/AIVisualFeedback/AIContextMarkVisualFeedbackEditorProfile", order = 1)]
    public class AIContextMarkVisualFeedbackEditorProfile : AbstractCreationWizardEditorProfile
    {
        public override void OnEnable()
        {
            base.OnEnable();
            this.InitModule<GenericInformation>(false, true, false);
            this.InitModule<AIContextMarkTypePicker>(false, true, false);
            this.InitModule<SingleMarkCreator>(false, false, false);
            this.InitModule<AlternanceMarkCreator>(false, false, false);
        }

        public override void OnGenerationEnd()
        {

        }
    }

}

