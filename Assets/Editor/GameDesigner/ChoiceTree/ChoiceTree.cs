using System;
using System.Collections.Generic;

namespace Editor_GameDesigner
{
    public class GameDesignerChoiceTree : TreePicker<Type>
    {
        private Dictionary<string, Type> Modules = new Dictionary<string, Type>()
        {
            {"Level//."+typeof(CreatePuzzleLevel).Name, typeof(CreatePuzzleLevel) },
            {"Level//."+typeof(CreateAdventureLevel).Name, typeof(CreateAdventureLevel) },
            {"Level//." + typeof(EditPuzzleLevel).Name, typeof(EditPuzzleLevel) },
            {"Level//."+typeof(CreateChunk).Name, typeof(CreateChunk) },
            {"LevelChunk//Environmnent//." + typeof(AddEnvironmentModel).Name, typeof(AddEnvironmentModel) },
            {"LevelChunk//Environmnent//Obstacles//." + typeof(AddNewObstacle).Name, typeof(AddNewObstacle) },
            {"Puzzle//AI//."+ typeof(CreateAI).Name, typeof(CreateAI) },
            {"Puzzle//AI//."+typeof(AddAI).Name, typeof(AddAI) },
            {"Puzzle//AI//." + typeof(AIModel).Name, typeof(AIModel) },
            {"Puzzle//AI//." + typeof(ExploreAI).Name, typeof(ExploreAI) },
            {"Puzzle//AI//." + typeof(AIManagerModuleWizard).Name, typeof(AIManagerModuleWizard) },
            {"Puzzle//AI//Behavior//."+typeof(CreateBehavior).Name, typeof(CreateBehavior) },
            {"Puzzle//AI//Behavior//."+typeof(EditBehavior).Name, typeof(EditBehavior) },
            {"Puzzle//Environment//." + typeof(GroundEffectAdd).Name, typeof(GroundEffectAdd) },
            {"Puzzle//TargetZone//." + typeof(CreateTargetZone).Name, typeof(CreateTargetZone) },
            {"Puzzle//TargetZone//." + typeof(EditTargetZone).Name, typeof(EditTargetZone) },
            {"Puzzle//TargetZone//." + typeof(AddTargetZone).Name, typeof(AddTargetZone) },
            {"Puzzle//TargetZone//." + typeof(ExploreTargetZone).Name, typeof(ExploreTargetZone) },
            {"Puzzle//Projectile//." + typeof(CreateProjectile).Name, typeof(CreateProjectile) },
            {"Puzzle//Projectile//." + typeof(EditProjectile).Name, typeof(EditProjectile) },
            {"Puzzle//Projectile//." + typeof(ExploreProjectile).Name, typeof(ExploreProjectile) },
            {"Puzzle//Projectile//." + typeof(ProjectileModel).Name, typeof(ProjectileModel) },
            {"Puzzle//InteractiveObject//." + typeof(CreateInteractiveObject).Name, typeof(CreateInteractiveObject) },
            {"Puzzle//InteractiveObject//." + typeof(InteractiveObjectModuleWizard).Name, typeof(InteractiveObjectModuleWizard) },
            {"Puzzle//AttractiveObject//." + typeof(AttractiveObjectModel).Name, typeof(AttractiveObjectModel) },
            {"Puzzle//AttractiveObject//." + typeof(CreateAttractiveObject).Name, typeof(CreateAttractiveObject) },
            {"Puzzle//AttractiveObject//." + typeof(EditAttractiveObject).Name, typeof(EditAttractiveObject) },
            {"Puzzle//AttractiveObject//." + typeof(ExploreAttractiveObject).Name, typeof(ExploreAttractiveObject) },
            {"Puzzle//PlayerAction//." + typeof(ExplorePlayerActions).Name, typeof(ExplorePlayerActions) },
            {"Puzzle//PlayerAction//." + typeof(CreatePlayerActions).Name, typeof(CreatePlayerActions) },
            {"Puzzle//ContextMark//." + typeof(ExploreContextMark).Name, typeof(ExploreContextMark) },
            {"Puzzle//InteractiveObjects//." + typeof(ExploreRepelableObject).Name, typeof(ExploreRepelableObject) },
            {"Puzzle//InteractiveObjects//." + typeof(CreateRepelableObject).Name, typeof(CreateRepelableObject) },
            {"Puzzle//InteractiveObjects//." + typeof(EditRepelableObject).Name, typeof(EditRepelableObject) },
            {"Adventure//POI//." + typeof(CreatePOI).Name, typeof(CreatePOI) },
            {"Adventure//POI//." + typeof(EditPOI).Name, typeof(EditPOI) },
            {"Adventure//POI//." + typeof(POIModel).Name, typeof(POIModel) },
            {"Adventure//POI//." + typeof(ExplorePOI).Name, typeof(ExplorePOI) },
            {"Adventure//POI//Modules//." + typeof(POIModuleWizard).Name, typeof(POIModuleWizard) },
            {"Adventure//Discussion//Modules//." + typeof(CreateDiscussionTree).Name, typeof(CreateDiscussionTree) },
            {"Core//Animation//." + typeof(CreateAnimation).Name, typeof(CreateAnimation) },
            {"Configuration//." + typeof(CutsceneConfigurationModule).Name, typeof(CutsceneConfigurationModule) },
            {"Configuration//." + typeof(DiscussionTreeConfigurationModule).Name, typeof(DiscussionTreeConfigurationModule) },
            {"Configuration//." + typeof(ItemConfigurationModule).Name, typeof(ItemConfigurationModule) },
            {"Configuration//." + typeof(PointOfInterestConfigurationModule).Name, typeof(PointOfInterestConfigurationModule) },
            {"Configuration//." + typeof(AnimationConfigurationModule).Name, typeof(AnimationConfigurationModule) },
            {"Configuration//." + typeof(ChunkZonesSceneConfigurationModule).Name, typeof(ChunkZonesSceneConfigurationModule) },
            {"Configuration//." + typeof(LevelHierarchyConfigurationModule).Name, typeof(LevelHierarchyConfigurationModule) },
            {"Configuration//." + typeof(LevelZonesSceneConfigurationModule).Name, typeof(LevelZonesSceneConfigurationModule) },
            {"Configuration//." + typeof(TimelineConfigurationModule).Name, typeof(TimelineConfigurationModule) },
            {"Configuration//." + typeof(AIComponentsConfigurationModule).Name, typeof(AIComponentsConfigurationModule) },
            {"Configuration//." + typeof(ContextMarkVisualFeedbackConfigurationModule).Name, typeof(ContextMarkVisualFeedbackConfigurationModule) },
            {"Configuration//." + typeof(AttractiveObjectConfigurationModule).Name, typeof(AttractiveObjectConfigurationModule) },
            {"Configuration//." + typeof(DottedLineConfigurationModule).Name, typeof(DottedLineConfigurationModule) },
            {"Configuration//." + typeof(RepelableObjectsConfigurationModule).Name, typeof(RepelableObjectsConfigurationModule) },
            {"Configuration//." + typeof(LevelConfigurationModule).Name, typeof(LevelConfigurationModule) },
            {"Configuration//." + typeof(PlayerActionConfigurationModule).Name, typeof(PlayerActionConfigurationModule) },
            {"Configuration//." + typeof(ProjectileConfigurationModule).Name, typeof(ProjectileConfigurationModule) },
            {"Configuration//." + typeof(RangeTypeConfigurationModule).Name, typeof(RangeTypeConfigurationModule) },
            {"Configuration//." + typeof(SelectionWheelNodeConfigurationModule).Name, typeof(SelectionWheelNodeConfigurationModule) },
            {"Configuration//." + typeof(TargetZonesConfigurationModule).Name, typeof(TargetZonesConfigurationModule) },
            {"Configuration//." + typeof(DiscussionRepertoireConfigurationModule).Name, typeof(DiscussionRepertoireConfigurationModule) },
        };

        public override Dictionary<string, Type> Configurations => this.Modules;

        private GameDesignerEditorProfile GameDesignerEditorProfileRef;

        public GameDesignerChoiceTree(GameDesignerEditorProfile gameDesignerEditorProfileRef)
        {
            GameDesignerEditorProfileRef = gameDesignerEditorProfileRef;
        }

        protected override void OnSelectionChange()
        {
            base.OnSelectionChange();
            this.GameDesignerEditorProfileRef.ChangeCurrentModule((IGameDesignerModule)Activator.CreateInstance(this.GetSelectedConf()));
        }
    }

}
