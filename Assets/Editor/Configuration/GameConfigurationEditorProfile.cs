using ConfigurationEditor;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GameConfigurationEditorProfile", menuName = "Configuration/GameConfigurationEditorProfile", order = 1)]
    public class GameConfigurationEditorProfile : MultipleChoiceHeaderTab<IGenericConfigurationEditor>
    {

        [SerializeField]
        private Dictionary<string, MultipleChoiceHeaderTabSelectionProfile> selection;

        [SerializeField]
        private Dictionary<string, IGenericConfigurationEditor> configurations;

        public GameConfigurationEditorProfile()
        {
            this.selection = new Dictionary<string, MultipleChoiceHeaderTabSelectionProfile>() {
                { ComputeSelectionKey(typeof(ProjectileInherentData)), new MultipleChoiceHeaderTabSelectionProfile("PROJ")},
                { ComputeSelectionKey(typeof(TargetZoneInherentData)), new MultipleChoiceHeaderTabSelectionProfile("TARG")},
                { ComputeSelectionKey(typeof(AttractiveObjectActionInherentData)), new MultipleChoiceHeaderTabSelectionProfile("ATTR_OBJ") },
                { ComputeSelectionKey(typeof(LevelConfigurationData)), new MultipleChoiceHeaderTabSelectionProfile("LEVEL") },
                { ComputeSelectionKey(typeof(SelectionWheelNodeConfigurationData)), new MultipleChoiceHeaderTabSelectionProfile("WHEEL_ACT") },
                { ComputeSelectionKey(typeof(GenericPuzzleAIComponents)), new MultipleChoiceHeaderTabSelectionProfile("AI") },
                { ComputeSelectionKey(typeof(PlayerActionInherentData)), new MultipleChoiceHeaderTabSelectionProfile("PLA_ACT")},
                { ComputeSelectionKey(typeof(ContextMarkVisualFeedbackInherentData)), new MultipleChoiceHeaderTabSelectionProfile("CTX_MARK")}
             };
            this.configurations = new Dictionary<string, IGenericConfigurationEditor>() {
                {  ComputeSelectionKey(typeof(ProjectileInherentData)), new GenericConfigurationEditor<LaunchProjectileId, ProjectileInherentData>("t:ProjectileConfiguration")},
                  {  ComputeSelectionKey(typeof(TargetZoneInherentData)), new GenericConfigurationEditor<TargetZoneID, TargetZoneInherentData>("t:TargetZonesConfiguration")},
                  {  ComputeSelectionKey(typeof(AttractiveObjectActionInherentData)), new GenericConfigurationEditor<AttractiveObjectId, AttractiveObjectInherentConfigurationData>("t:AttractiveObjectConfiguration")},
                  {  ComputeSelectionKey(typeof(LevelConfigurationData)), new GenericConfigurationEditor<LevelZonesID, LevelConfigurationData>("t:LevelConfiguration")},
                  {  ComputeSelectionKey(typeof(SelectionWheelNodeConfigurationData)), new GenericConfigurationEditor<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>("t:SelectionWheelNodeConfiguration")},
                  {  ComputeSelectionKey(typeof(GenericPuzzleAIComponents)), new GenericConfigurationEditor<AiID, AIBehaviorInherentData>("t:AIComponentsConfiguration")},
                  { ComputeSelectionKey(typeof(PlayerActionInherentData)), new GenericConfigurationEditor<PlayerActionId, PlayerActionInherentData>("t:PlayerActionConfiguration")},
                { ComputeSelectionKey(typeof(ContextMarkVisualFeedbackInherentData)), new GenericConfigurationEditor<AiID, ContextMarkVisualFeedbackInherentData>("t:"+typeof(ContextMarkVisualFeedbackConfiguration).Name)}
            };
        }

        public override Dictionary<string, MultipleChoiceHeaderTabSelectionProfile> ConfigurationSelection => this.selection;

        public override Dictionary<string, IGenericConfigurationEditor> Configurations => this.configurations;
    }

}
