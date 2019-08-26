using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GameCreationWizardEditorProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile", order = 1)]
public class GameCreationWizardEditorProfile : TreeChoiceHeaderTab<ICreationWizardEditor>
{
    [SerializeField]
    private Dictionary<string, ICreationWizardEditor> configurations = GameCreationWizardEditorProfileChoiceTree.configurations;

    public override Dictionary<string, ICreationWizardEditor> Configurations => this.configurations;

}
