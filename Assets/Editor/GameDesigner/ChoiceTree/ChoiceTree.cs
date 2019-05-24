using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Editor_GameDesigner
{
    public class GameDesignerChoiceTree : TreePicker<Type>
    {
        private Dictionary<string, Type> Modules = new Dictionary<string, Type>()
        {
            {"Environment//." + typeof(GroundEffectAdd).Name, typeof(GroundEffectAdd) },
            {"Level//."+typeof(CreatePuzzleLevel).Name, typeof(CreatePuzzleLevel) },
            {"AI//."+ typeof(CreateAI).Name, typeof(CreateAI) },
            {"AI//."+typeof(AddAI).Name, typeof(AddAI) },
            {"AI//." + typeof(AIModel).Name, typeof(AIModel) },
            {"AI//Behavior//."+typeof(CreateBehavior).Name, typeof(CreateBehavior) },
            {"AI//Behavior//."+typeof(EditBehavior).Name, typeof(EditBehavior) },
            {"TargetZone//." + typeof(CreateTargetZone).Name, typeof(CreateTargetZone) },
            {"TargetZone//." + typeof(EditTargetZone).Name, typeof(EditTargetZone) },
            {"TargetZone//." + typeof(AddTargetZone).Name, typeof(AddTargetZone) }
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
