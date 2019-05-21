using Editor_AIContextMarkVisualFeedbackCreationWizardEditor;
using Editor_PuzzleGameCreationWizard;
using Editor_PlayerActionVariantCreationWizardEditor;
using Editor_AIContextMarkGameConfigWizard;
using System.Collections.Generic;
using UnityEngine;
using Editor_PuzzleLevelCreationWizard;
using Editor_AICreationObjectCreationWizard;

[System.Serializable]
[CreateAssetMenu(fileName = "PuzzleCreationWizardEditorProfile", menuName = "CreationWizard/PuzzleCreationWizardEditorProfile", order = 1)]
public class PuzzleCreationWizardEditorProfile : TreeChoiceHeaderTab<ICreationWizardEditor<AbstractCreationWizardEditorProfile>>
{

    [SerializeField]
    private Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> configurations;


    public PuzzleCreationWizardEditorProfile()
    {
        this.configurations = new Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>>()
        {
            {nameof(AttractiveObjectVariantCreationWizardV2), new AttractiveObjectVariantCreationWizardV2() },
            {nameof(PlayerActionVariantCreationWizard), new PlayerActionVariantCreationWizard() },
            {nameof(AIContextMarkVisualFeedbackCreationWizard), new AIContextMarkVisualFeedbackCreationWizard() },
            {nameof(AIContextMarkGameConfigWizard), new AIContextMarkGameConfigWizard() },
            {nameof(PuzzleLevelCreationWizard), new PuzzleLevelCreationWizard() },
            {nameof(AIObjectCreationWizard), new AIObjectCreationWizard() }
        };
    }

    public override Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> Configurations => this.configurations;
}
