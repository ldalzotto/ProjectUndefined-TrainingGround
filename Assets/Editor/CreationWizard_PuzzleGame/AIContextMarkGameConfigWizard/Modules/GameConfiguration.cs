using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using System.Collections.Generic;
using CreationWizard;

namespace Editor_AIContextMarkGameConfigWizard
{
    public class GameConfiguration : CreationModuleComponent
    {
        public ContextMarkVisualFeedbackConfiguration ContextMarkVisualFeedbackConfiguration;

        public GameConfiguration(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject)
        {
            if (this.ContextMarkVisualFeedbackConfiguration == null)
            {
                this.ContextMarkVisualFeedbackConfiguration = AssetFinder.SafeSingleAssetFind<ContextMarkVisualFeedbackConfiguration>("t:" + typeof(ContextMarkVisualFeedbackConfiguration).Name);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.ContextMarkVisualFeedbackConfiguration)));
        }

    }

}
