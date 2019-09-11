using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{

    public class PuzzleGameConfigurationManager : MonoBehaviour
    {
        public PuzzleGameConfiguration PuzzleGameConfiguration;

        public void Init()
        {
            PuzzleGameConfiguration.LevelConfiguration.Init(PuzzleGameConfiguration.PlayerActionConfiguration);
        }

        public Dictionary<LaunchProjectileID, LaunchProjectileInherentData> ProjectileConf()
        {
            return PuzzleGameConfiguration.LaunchProjectileConfiguration.ConfigurationInherentData;
        }

        public Dictionary<TargetZoneID, TargetZoneInherentData> TargetZonesConfiguration()
        {
            return PuzzleGameConfiguration.TargetZoneConfiguration.ConfigurationInherentData;
        }

        public Dictionary<PlayerActionId, PlayerActionInherentData> PlayerActionConfiguration()
        {
            return PuzzleGameConfiguration.PlayerActionConfiguration.ConfigurationInherentData;
        }

        public Dictionary<AttractiveObjectId, AttractiveObjectInherentConfigurationData> AttractiveObjectsConfiguration()
        {
            return PuzzleGameConfiguration.AttractiveObjectConfiguration.ConfigurationInherentData;
        }
        public Dictionary<LevelZonesID, LevelConfigurationData> LevelConfiguration()
        {
            return PuzzleGameConfiguration.LevelConfiguration.ConfigurationInherentData;
        }
        public Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData> SelectionWheelNodeConfiguration()
        {
            return PuzzleGameConfiguration.SelectionWheelNodeConfiguration.ConfigurationInherentData;
        }

        public Dictionary<RangeTypeID, RangeTypeInherentConfigurationData> RangeTypeConfiguration()
        {
            return PuzzleGameConfiguration.RangeTypeConfiguration.ConfigurationInherentData;
        }

        public Dictionary<DottedLineID, DottedLineInherentData> DottedLineConfiguration()
        {
            return PuzzleGameConfiguration.DottedLineConfiguration.ConfigurationInherentData;
        }

        public Dictionary<ObjectRepelID, ObjectRepelInherentData> RepelableObjectsConfiguration()
        {
            return PuzzleGameConfiguration.ObjectRepelConfiguration.ConfigurationInherentData;
        }
        public Dictionary<DisarmObjectID, DisarmObjectInherentData> DisarmObjectsConfiguration()
        {
            return PuzzleGameConfiguration.DisarmObjectConfiguration.ConfigurationInherentData;
        }

        public Dictionary<PuzzleCutsceneID, PuzzleCutsceneInherentData> PuzzleCutsceneConfiguration()
        {
            return PuzzleGameConfiguration.PuzzleCutsceneConfiguration.ConfigurationInherentData;
        }

        public Dictionary<ActionInteractableObjectID, ActionInteractableObjectInherentData> ActionInteractableObjectConfiguration()
        {
            return PuzzleGameConfiguration.ActionInteractableObjectConfiguration.ConfigurationInherentData;
        }

        public Dictionary<GrabObjectID, GrabObjectInherentData> GrabObjectConfiguration()
        {
            return PuzzleGameConfiguration.GrabObjectConfiguration.ConfigurationInherentData;
        }

        public Dictionary<NearPlayerGameOverTriggerID, NearPlayerGameOverTriggerInherentData> NearPlayerGameOverTriggerConfiguration()
        {
            return PuzzleGameConfiguration.NearPlayerGameOverTriggerConfiguration.ConfigurationInherentData;
        }

        public Dictionary<RangeTypeObjectDefinitionID, RangeTypeObjectDefinitionInherentData> RangeTypeObjectDefinitionConfiguration()
        {
            return PuzzleGameConfiguration.RangeTypeObjectDefinitionConfiguration.ConfigurationInherentData;
        }

        public Dictionary<InteractiveObjectTypeDefinitionID, InteractiveObjectTypeDefinitionInherentData> InteractiveObjectTypeDefinitionConfiguration()
        {
            return PuzzleGameConfiguration.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData;
        }
        //------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------
        public Dictionary<AIObjectTypeDefinitionID, AIObjectTypeDefinitionInherentData> AIObjectTypeDefinitionConfiguration()
        {
            return PuzzleGameConfiguration.AIObjectTypeDefinitionConfiguration.ConfigurationInherentData;
        }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------
        public Dictionary<AIObjectID, AIObjectInherentData> AIObjectConfiguration()
        {
            return PuzzleGameConfiguration.AIObjectConfiguration.ConfigurationInherentData;
        }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
// </auto-generated>
//------------------------------------------------------------------------------
        public Dictionary<AIPatrolGraphID, AIPatrolGraphInherentData> AIPatrolGraphConfiguration()
        {
            return PuzzleGameConfiguration.AIPatrolGraphConfiguration.ConfigurationInherentData;
        }

//${addNewEntry}
    }



}
