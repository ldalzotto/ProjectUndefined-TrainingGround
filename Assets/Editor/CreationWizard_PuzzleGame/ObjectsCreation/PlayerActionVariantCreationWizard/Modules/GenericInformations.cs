using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    [System.Serializable]
    public class GenericInformations : CreationModuleComponent
    {
        [SearchableEnum]
        public AttractiveObjectId AttractiveObjectId;

        [SearchableEnum]
        public PlayerActionId PlayerActionId;

        [SearchableEnum]
        public LevelZonesID LevelZonesID;

        [SearchableEnum]
        public SelectionWheelNodeConfigurationId SelectionWheelNodeConfigurationId;

        public PathConfiguration PathConfiguration;

        public GenericInformations(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "Generic informations : ";

        protected override string headerDescriptionLabel => "Informations about ID and generation path.";
        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            Editor.CreateEditor(this).OnInspectorGUI();
        }
    }

    [System.Serializable]
    public class PathConfiguration
    {
        public string AttractiveObjectPlayerActionConfigurationPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/AttractiveObjectConfiguration/AttractiveObjectInherentConfigurationData";
        public string WheelActionConfigurationPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/SelectionNodeConfiguration/SelectionNodeConfigurationData";
    }

}
