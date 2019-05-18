using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using System.Collections.Generic;

namespace Editor_PuzzleGameCreationWizard
{
    [System.Serializable]
    public class Configurationretrieval : CreationModuleComponent
    {
        private AttractiveObjectConfiguration attractiveObjectConfiguration;

        public Configurationretrieval(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public AttractiveObjectConfiguration AttractiveObjectConfiguration { get => attractiveObjectConfiguration; set => attractiveObjectConfiguration = value; }

        protected override string foldoutLabel => "Game configuration : ";

        protected override string headerDescriptionLabel => "Global game configuration.";
        public override void ResetEditor()
        {

        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Attractive object configuration : ", this.attractiveObjectConfiguration, typeof(AttractiveObjectConfiguration), false);
            EditorGUI.EndDisabledGroup();

            if (this.attractiveObjectConfiguration == null)
            {
                this.attractiveObjectConfiguration = AssetFinder.SafeSingleAssetFind<AttractiveObjectConfiguration>("t:" + typeof(AttractiveObjectConfiguration).ToString());
            }
        }
    }
}
