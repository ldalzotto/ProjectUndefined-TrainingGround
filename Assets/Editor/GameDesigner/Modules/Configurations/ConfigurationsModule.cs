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
    public class RepelableObjectsConfigurationModule : ConfigurationModule<RepelableObjectsConfiguration, RepelableObjectID, RepelableObjectsInherentData> { }
    public class LevelConfigurationModule : ConfigurationModule<LevelConfiguration, LevelZonesID, LevelConfigurationData> { }
    public class PlayerActionConfigurationModule : ConfigurationModule<PlayerActionConfiguration, PlayerActionId, PlayerActionInherentData> { }
    public class ProjectileConfigurationModule : ConfigurationModule<ProjectileConfiguration, LaunchProjectileId, ProjectileInherentData> { }
    public class RangeTypeConfigurationModule : ConfigurationModule<RangeTypeConfiguration, RangeTypeID, RangeTypeInherentConfigurationData> { }
    public class SelectionWheelNodeConfigurationModule : ConfigurationModule<RTPuzzle.SelectionWheelNodeConfiguration, SelectionWheelNodeConfigurationId, RTPuzzle.SelectionWheelNodeConfigurationData> { }
    public class TargetZonesConfigurationModule : ConfigurationModule<TargetZonesConfiguration, TargetZoneID, TargetZoneInherentData> { }
    public class DisarmObjectConfigurationModule : ConfigurationModule<DisarmObjectConfiguration, DisarmObjectID, DisarmObjectInherentData> { }
    public class ActionInteractableObjectConfigurationModule : ConfigurationModule<ActionInteractableObjectConfiguration, ActionInteractableObjectID, ActionInteractableObjectInherentData> { }

    public class DiscussionRepertoireConfigurationModule : IGameDesignerModule
    {
        [SerializeField]
        private DiscussionTextRepertoire DiscussionTextRepertoire;
        [SerializeField]
        private Editor CachedEditor;

        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.CachedEditor.OnInspectorGUI();
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
            this.DiscussionTextRepertoire = AssetFinder.SafeSingleAssetFind<DiscussionTextRepertoire>("t:" + typeof(DiscussionTextRepertoire).Name);
            this.CachedEditor = Editor.CreateEditor(this.DiscussionTextRepertoire);
        }
    }

}
