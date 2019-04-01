using UnityEngine;
using System.Collections;
using RTPuzzle;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    [System.Serializable]
    public class AttractiveObjectInherentDataModule : CreateableScriptableObjectComponent<AttractiveObjectInherentConfigurationData>
    {
        public AttractiveObjectInherentDataModule(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => "Generated attractive object configuration : ";

        protected override string foldoutLabel => "Object game configuration : ";

        protected override string headerDescriptionLabel => "The configuration of the attractive object.";
    }
}
