using Editor_AttractiveObjectVariantWizardEditor;
using Editor_PlayerActionVariantCreationWizardEditor;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CreationWizardEditorProfile", menuName = "CreationWizardEditorProfile", order = 1)]
public class CreationWizardEditorProfile : MultipleChoiceHeaderTab<ICreationWizardEditor<AbstractCreationWizardEditorProfile>>
{

    [SerializeField]
    private Dictionary<string, MultipleChoiceHeaderTabSelectionProfile> selection;

    [SerializeField]
    private Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> configurations;

    public CreationWizardEditorProfile()
    {
        this.selection = new Dictionary<string, MultipleChoiceHeaderTabSelectionProfile>() {
            {ComputeSelectionKey(typeof(AttractiveObjectVariantCreationWizardV2)), new MultipleChoiceHeaderTabSelectionProfile("ATTR_OBJ") },
            {ComputeSelectionKey(typeof(PlayerActionVariantCreationWizard)), new MultipleChoiceHeaderTabSelectionProfile("PLA_ACT") }
        };
        this.configurations = new Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>>()
        {
            {ComputeSelectionKey(typeof(AttractiveObjectVariantCreationWizardV2)), new AttractiveObjectVariantCreationWizardV2() },
            {ComputeSelectionKey(typeof(PlayerActionVariantCreationWizard)), new PlayerActionVariantCreationWizard() }
        };
    }

    public override Dictionary<string, MultipleChoiceHeaderTabSelectionProfile> ConfigurationSelection => this.selection;

    public override Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> Configurations => this.configurations;


}
