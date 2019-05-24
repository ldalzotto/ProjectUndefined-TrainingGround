using Editor_PuzzleGameCreationWizard;
using System.Collections.Generic;
using UnityEngine;
using Editor_PuzzleLevelCreationWizard;
using Editor_AICreationObjectCreationWizard;
using Editor_AIBehaviorCreationWizard;
using Editor_TargetZoneCreationWizard;

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
            {nameof(AIObjectCreationWizard), new AIObjectCreationWizard() },
            {nameof(AIBehaviorCreationWizard), new AIBehaviorCreationWizard() },
            {nameof(TargetZoneCreationWizard), new TargetZoneCreationWizard() }
        };
    }

    public override Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> Configurations => this.configurations;

}
