using Editor_AIContextMarkVisualFeedbackCreationWizardEditor;
using Editor_AttractiveObjectVariantWizardEditor;
using Editor_PlayerActionVariantCreationWizardEditor;
using Editor_AIContextMarkGameConfigWizard;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CreationWizardEditorProfile", menuName = "CreationWizardEditorProfile", order = 1)]
public class CreationWizardEditorProfile : TreeChoiceHeaderTab<ICreationWizardEditor<AbstractCreationWizardEditorProfile>>
{

    [SerializeField]
    private Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> configurations;


    public CreationWizardEditorProfile()
    {
        this.configurations = new Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>>()
        {
            {nameof(AttractiveObjectVariantCreationWizardV2), new AttractiveObjectVariantCreationWizardV2() },
            {nameof(PlayerActionVariantCreationWizard), new PlayerActionVariantCreationWizard() },
            {nameof(AIContextMarkVisualFeedbackCreationWizard), new AIContextMarkVisualFeedbackCreationWizard() },
            {nameof(AIContextMarkGameConfigWizard), new AIContextMarkGameConfigWizard() }
        };
    }

    public override Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> Configurations => this.configurations;
}
