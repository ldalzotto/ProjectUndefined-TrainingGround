using RTPuzzle;
using UnityEditor;
using UnityEngine;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    public class AIFeedbackMarkCreation : CreateablePrefabComponent<GameObject, AIFeedbackMarkType>
    {
        
        public AIFeedbackMarkCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "AI feedback mark : ";

        protected override string headerDescriptionLabel => "The visual feedback on top of AI when he is influenced by this action.";

        public override void ResetEditor()
        {
        }
        
    }

}
