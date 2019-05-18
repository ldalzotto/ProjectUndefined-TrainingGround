using RTPuzzle;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_AIContextMarkGameConfigWizard
{
    public class GenericInformations : CreationModuleComponent
    {

        [SearchableEnum]
        public AiID AiID;
        public string BaseName;
        public PathConfiguration PathConfiguration;

        public GenericInformations(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.AiID)), new GUIContent("The associated AI to give the newly create configuration : "));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.BaseName)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.PathConfiguration)), true);
        }
    }

    [System.Serializable]
    public class PathConfiguration
    {
        public string AIContextMarkConfigurationPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/AIContextMarkVisualFeedback/AiContextMarkVisualFeedbackData";
    }

}
