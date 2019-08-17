using AdventureGame;
using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    public class CutsceneConfigurationModule : ConfigurationModule<CutsceneConfiguration, CutsceneId, CutsceneInherentData> { }
    public class PuzzleCutsceneConfigurationModule : ConfigurationModule<PuzzleCutsceneConfiguration, PuzzleCutsceneId, PuzzleCutsceneInherentData> { }
    public class DiscussionTreeConfigurationModule : ConfigurationModule<DiscussionTreeConfiguration, DiscussionTreeId, DiscussionTree> { }
    public class ItemConfigurationModule : ConfigurationModule<ItemConfiguration, ItemID, ItemInherentData> { }
    public class PointOfInterestConfigurationModule : ConfigurationModule<PointOfInterestConfiguration, PointOfInterestId, PointOfInterestInherentData> { }
    public class AnimationConfigurationModule : ConfigurationModule<AnimationConfiguration, AnimationID, AnimationConfigurationData> { }
    public class ChunkZonesSceneConfigurationModule : ConfigurationModule<ChunkZonesSceneConfiguration, LevelZoneChunkID, LevelZonesSceneConfigurationData> { }
    public class LevelHierarchyConfigurationModule : ConfigurationModule<LevelHierarchyConfiguration, LevelZonesID, LevelHierarchyConfigurationData> { }
    public class LevelZonesSceneConfigurationModule : ConfigurationModule<LevelZonesSceneConfiguration, LevelZonesID, LevelZonesSceneConfigurationData> { }
    public class TimelineConfigurationModule : ConfigurationModule<TimelineConfiguration, TimelineID, TimelineInitializerScriptableObject> { }
    public class AIComponentsConfigurationModule : ConfigurationModule<AIComponentsConfiguration, AiID, AIBehaviorInherentData> { }
    public class ContextMarkVisualFeedbackConfigurationModule : ConfigurationModule<ContextMarkVisualFeedbackConfiguration, AiID, ContextMarkVisualFeedbackInherentData> { }
    public class AttractiveObjectConfigurationModule : ConfigurationModule<AttractiveObjectConfiguration, AttractiveObjectId, AttractiveObjectInherentConfigurationData> { }
    public class DottedLineConfigurationModule : ConfigurationModule<DottedLineConfiguration, DottedLineID, DottedLineInherentData> { }
    public class RepelableObjectsConfigurationModule : ConfigurationModule<ObjectRepelConfiguration, ObjectRepelID, ObjectRepelInherentData> { }
    public class LevelConfigurationModule : ConfigurationModule<LevelConfiguration, LevelZonesID, LevelConfigurationData> { }
    public class PlayerActionConfigurationModule : ConfigurationModule<PlayerActionConfiguration, PlayerActionId, PlayerActionInherentData> { }
    public class LaunchProjectileConfigurationModule : ConfigurationModule<LaunchProjectileConfiguration, LaunchProjectileID, LaunchProjectileInherentData> { }
    public class RangeTypeConfigurationModule : ConfigurationModule<RangeTypeConfiguration, RangeTypeID, RangeTypeInherentConfigurationData> { }
    public class SelectionWheelNodeConfigurationModule : ConfigurationModule<RTPuzzle.SelectionWheelNodeConfiguration, SelectionWheelNodeConfigurationId, RTPuzzle.SelectionWheelNodeConfigurationData> { }
    public class TargetZoneConfigurationModule : ConfigurationModule<TargetZoneConfiguration, TargetZoneID, TargetZoneInherentData> { }
    public class DisarmObjectConfigurationModule : ConfigurationModule<DisarmObjectConfiguration, DisarmObjectID, DisarmObjectInherentData> { }
    public class ActionInteractableObjectConfigurationModule : ConfigurationModule<ActionInteractableObjectConfiguration, ActionInteractableObjectID, ActionInteractableObjectInherentData> { }
    public class InputConfigurationModule : ConfigurationModule<InputConfiguration, InputID, InputConfigurationInherentData> { }
    public class DiscussionTextConfigurationModule : ConfigurationModule<DiscussionTextConfiguration, DiscussionTextID, DiscussionTextInherentData> { }
    public class TutorialStepConfigurationModule : ConfigurationModule<TutorialStepConfiguration, TutorialStepID, TutorialStepInherentData> { }
    public class NearPlayerGameOverTriggerConfigurationModule : ConfigurationModule<NearPlayerGameOverTriggerConfiguration, NearPlayerGameOverTriggerID, NearPlayerGameOverTriggerInherentData> { }
    //------------------------------------------------------------------------------
    // <auto-generated>
    //     This code was generated by a tool.
    // </auto-generated>
    //------------------------------------------------------------------------------
    public class GrabObjectConfigurationModule : ConfigurationModule<GrabObjectConfiguration, GrabObjectID, GrabObjectInherentData> { }
//${addNewEntry}

}
