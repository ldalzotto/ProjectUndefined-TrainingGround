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

        public override void ResetEditor()
        {
        }
        
    }

}
