using RTPuzzle;
using UnityEditor;
using UnityEngine;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    public class AIFeedbackMarkCreation : CreateablePrefabComponent<GameObject, AIFeedbackMarkType>
    {
        public Material AIFeedbackVertexLitMaterial;

        public AIFeedbackMarkCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "AI feedback mark : ";

        protected override string headerDescriptionLabel => "The visual feedback on top of AI when he is influenced by this action.";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl()
        {
            base.OnInspectorGUIImpl();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Vertex lit material : ", this.AIFeedbackVertexLitMaterial, typeof(Material), false);
            EditorGUI.EndDisabledGroup();

            if(this.AIFeedbackVertexLitMaterial == null)
            {
                this.AIFeedbackVertexLitMaterial = AssetFinder.SafeSingleAssetFind<Material>("VertexUnlitInstanciatedMaterial");
            }
        }
    }

}
