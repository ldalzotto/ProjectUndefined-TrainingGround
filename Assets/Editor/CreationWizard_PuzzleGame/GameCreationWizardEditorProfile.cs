using Editor_MainGameCreationWizard;
using System.Collections.Generic;
using UnityEngine;
using Editor_PuzzleLevelCreationWizard;
using Editor_AICreationObjectCreationWizard;
using Editor_AIBehaviorCreationWizard;
using Editor_TargetZoneCreationWizard;
using Editor_ProjectileCreationWizard;
using Editor_PlayerActionCreationWizard;
using Editor_POICreationWizard;
using Editor_AttractiveObjectCreationWizard;
using Editor_RepelableObjectCreationWizard;
using Editor_AdventureBaseLevelCreationWizard;
using Editor_LevelChunkCreationWizard;
using Editor_AnimationCreationWizard;

[System.Serializable]
[CreateAssetMenu(fileName = "GameCreationWizardEditorProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile", order = 1)]
public class GameCreationWizardEditorProfile : TreeChoiceHeaderTab<ICreationWizardEditor<AbstractCreationWizardEditorProfile>>
{

    [SerializeField]
    private Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> configurations;


    public GameCreationWizardEditorProfile()
    {
        this.configurations = new Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>>()
        {
            {nameof(PuzzleLevelCreationWizard), new PuzzleLevelCreationWizard() },
            {nameof(AIObjectCreationWizard), new AIObjectCreationWizard() },
            {nameof(AIBehaviorCreationWizard), new AIBehaviorCreationWizard() },
            {nameof(TargetZoneCreationWizard), new TargetZoneCreationWizard() },
            {nameof(ProjectileCreationWizard), new ProjectileCreationWizard() },
            {nameof(PlayerActionCreationWizard), new PlayerActionCreationWizard() },
            {nameof(AttractiveObjectCreationWizard), new AttractiveObjectCreationWizard() },
            {nameof(POICreationWizard), new POICreationWizard() },
            {nameof(RepelableObjectCreationWizard), new RepelableObjectCreationWizard() },
            {nameof(AdventureBaseLevelCreationWizard), new AdventureBaseLevelCreationWizard() },
            {nameof(LevelChunkCreationWizard), new LevelChunkCreationWizard() },
            {nameof(AnimationCreationWizard), new AnimationCreationWizard() }


        };
    }

    public override Dictionary<string, ICreationWizardEditor<AbstractCreationWizardEditorProfile>> Configurations => this.configurations;

}
