using UnityEngine;
using System.Collections;
using RTPuzzle;
using Editor_AttractiveObjectVariantWizardEditor;
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

        protected override void OnInspectorGUIImpl()
        {
            Editor.CreateEditor(this).OnInspectorGUI();
        }
    }

}

