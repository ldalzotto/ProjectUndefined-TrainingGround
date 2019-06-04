using System;
using System.Collections.Generic;

namespace Editor_GameDesigner
{
    public class GameDesignerChoiceTree : TreePicker<Type>
    {
        private Dictionary<string, Type> Modules = new Dictionary<string, Type>()
        {
            {"Puzzle//Environment//." + typeof(GroundEffectAdd).Name, typeof(GroundEffectAdd) },
            {"Level//."+typeof(CreatePuzzleLevel).Name, typeof(CreatePuzzleLevel) },
            {"Level//." + typeof(EditPuzzleLevel).Name, typeof(EditPuzzleLevel) },
            {"LevelChunk//Environmnent//." + typeof(AddEnvironmentModel).Name, typeof(AddEnvironmentModel) },
            {"Puzzle//AI//."+ typeof(CreateAI).Name, typeof(CreateAI) },
            {"Puzzle//AI//."+typeof(AddAI).Name, typeof(AddAI) },
            {"Puzzle//AI//." + typeof(AIModel).Name, typeof(AIModel) },
            {"Puzzle//AI//." + typeof(ExploreAI).Name, typeof(ExploreAI) },
            {"Puzzle//AI//Behavior//."+typeof(CreateBehavior).Name, typeof(CreateBehavior) },
            {"Puzzle//AI//Behavior//."+typeof(EditBehavior).Name, typeof(EditBehavior) },
            {"Puzzle//TargetZone//." + typeof(CreateTargetZone).Name, typeof(CreateTargetZone) },
            {"Puzzle//TargetZone//." + typeof(EditTargetZone).Name, typeof(EditTargetZone) },
            {"Puzzle//TargetZone//." + typeof(AddTargetZone).Name, typeof(AddTargetZone) },
            {"Puzzle//TargetZone//." + typeof(ExploreTargetZone).Name, typeof(ExploreTargetZone) },
            {"Puzzle//Projectile//." + typeof(CreateProjectile).Name, typeof(CreateProjectile) },
            {"Puzzle//Projectile//." + typeof(EditProjectile).Name, typeof(EditProjectile) },
            {"Puzzle//PlayerAction//." + typeof(ExplorePlayerActions).Name, typeof(ExplorePlayerActions) },
            {"Puzzle//PlayerAction//." + typeof(CreatePlayerActions).Name, typeof(CreatePlayerActions) },
            {"Adventure//POI//." + typeof(CreatePOI).Name, typeof(CreatePOI) },
            {"Adventure//POI//." + typeof(EditPOI).Name, typeof(EditPOI) },
            {"Adventure//POI//." + typeof(POIModel).Name, typeof(POIModel) },
            {"Adventure//POI//." + typeof(ExplorePOI).Name, typeof(ExplorePOI) }
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
