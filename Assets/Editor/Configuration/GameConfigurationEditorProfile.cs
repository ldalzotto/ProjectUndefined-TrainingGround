using ConfigurationEditor;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GameConfigurationEditorProfile", menuName = "Configuration/GameConfigurationEditorProfile", order = 1)]
    public class GameConfigurationEditorProfile : ConfigurationEditorProfile
    {

        [SerializeField]
        private Dictionary<string, ConfigurationSelectionProfile> selection;

        [SerializeField]
        private Dictionary<string, IGenericConfigurationEditor> configurations;

        public GameConfigurationEditorProfile()
        {
            this.selection = new Dictionary<string, ConfigurationSelectionProfile>() {
                { ComputeSelectionKey(typeof(ProjectileInherentData)), new ConfigurationSelectionProfile("PROJ")},
                { ComputeSelectionKey(typeof(TargetZoneInherentData)), new ConfigurationSelectionProfile("TARG")},
                { ComputeSelectionKey(typeof(AttractiveObjectActionInherentData)), new ConfigurationSelectionProfile("ATTR_OBJ") },
                { ComputeSelectionKey(typeof(LevelConfigurationData)), new ConfigurationSelectionProfile("LEVEL") },
                { ComputeSelectionKey(typeof(SelectionWheelNodeConfigurationData)), new ConfigurationSelectionProfile("WHEEL_ACT") },
                { ComputeSelectionKey(typeof(AIComponents)), new ConfigurationSelectionProfile("AI") },
                { ComputeSelectionKey(typeof(PlayerActionInherentData)), new ConfigurationSelectionProfile("PLA_ACT")}
             };
            this.configurations = new Dictionary<string, IGenericConfigurationEditor>() {
                {  ComputeSelectionKey(typeof(ProjectileInherentData)), new GenericConfigurationEditor<LaunchProjectileId, ProjectileInherentData>("t:ProjectileConfiguration")},
                  {  ComputeSelectionKey(typeof(TargetZoneInherentData)), new GenericConfigurationEditor<TargetZoneID, TargetZoneInherentData>("t:TargetZonesConfiguration")},
                  {  ComputeSelectionKey(typeof(AttractiveObjectActionInherentData)), new GenericConfigurationEditor<AttractiveObjectId, AttractiveObjectInherentConfigurationData>("t:AttractiveObjectConfiguration")},
                  {  ComputeSelectionKey(typeof(LevelConfigurationData)), new GenericConfigurationEditor<LevelZonesID, LevelConfigurationData>("t:LevelConfiguration")},
                  {  ComputeSelectionKey(typeof(SelectionWheelNodeConfigurationData)), new GenericConfigurationEditor<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>("t:SelectionWheelNodeConfiguration")},
                  {  ComputeSelectionKey(typeof(AIComponents)), new GenericConfigurationEditor<AiID, AIBehaviorInherentData>("t:AIComponentsConfiguration")},
                  { ComputeSelectionKey(typeof(PlayerActionInherentData)), new GenericConfigurationEditor<PlayerActionId, PlayerActionInherentData>("t:PlayerActionConfiguration")}
            };
        }

        public override Dictionary<string, ConfigurationSelectionProfile> ConfigurationSelection => this.selection;

        public override Dictionary<string, IGenericConfigurationEditor> Configurations => this.configurations;
    }

}
