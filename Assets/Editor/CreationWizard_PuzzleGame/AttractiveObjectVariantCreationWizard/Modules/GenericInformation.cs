using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    [System.Serializable]
    public class GenericInformation : CreationModuleComponent
    {
        [SearchableEnum]
        public LevelZonesID LevelZoneID;

        [SearchableEnum]
        public AttractiveObjectId AttractiveObjectId;

        public string ObjectName;
        public AttractiveObjectType AttractiveObjectBasePrefab;
        public AIFeedbackMarkType AIFeedbackMarkBasePrefab;

        public PathConfiguration PathConfiguration;

        public GenericInformation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "Generic info : ";

        protected override string headerDescriptionLabel => "Informations about ID and generation path.";

        public override void ResetEditor()
        {
            this.ObjectName = "";
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject)
        {
            Editor.CreateEditor(this).OnInspectorGUI();
        }

        public override string ComputeErrorState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            if (string.IsNullOrEmpty(this.ObjectName))
            {
                return "Object name must not be empty.";
            }
            else if (string.IsNullOrEmpty(this.PathConfiguration.ObjectPrefabFolder))
            {
                return "ObjectPrefabFolder must not be empty";
            }
            else if (string.IsNullOrEmpty(this.PathConfiguration.ObjectConfigurationFolder))
            {
                return "ObjectConfigurationFolder must not be empty";
            }
            else if (string.IsNullOrEmpty(this.PathConfiguration.AIMarkPrefabFolder))
            {
                return "AIMarkPrefabFolder must not be empty";
            }
            return string.Empty;
        }
    }

}

