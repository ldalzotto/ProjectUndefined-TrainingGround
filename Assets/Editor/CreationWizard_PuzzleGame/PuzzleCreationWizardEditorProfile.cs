using Editor_PuzzleGameCreationWizard;
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
            {nameof(PuzzleLevelCreationWizard), new PuzzleLevelCreationWizard() },
            {nameof(AIObjectCreationWizard), new AIObjectCreationWizard() }
        };
    }

    public override Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> Configurations => this.configurations;
}
