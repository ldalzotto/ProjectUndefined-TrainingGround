using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Editor_GameDesigner
{
    public class ChoiceTree
    {
        private TreePickerPopup TreePickerPopup;

        private GameDesignerEditorProfile gameDesignerEditorProfile;

        private Dictionary<string, Type> Modules = new Dictionary<string, Type>()
        {
            {"Ground", typeof(GroundSetter) },
            {"AI//AddAI", typeof(AddAI) }
        };

        public ChoiceTree(ref GameDesignerEditorProfile gameDesignerEditorProfile)
        {
            this.TreePickerPopup = new TreePickerPopup(this.Modules.Keys.ToList(), () => { this.OnSelectionChange(this.TreePickerPopup.SelectedKey); },
                gameDesignerEditorProfile.GameDesignerTreePickerProfile.SelectedKey);
            this.TreePickerPopup.RepaintAction = () => { GUI.changed = true; };
            this.gameDesignerEditorProfile = gameDesignerEditorProfile;
        }

        public void GUITick()
        {
            this.TreePickerPopup.OnGUI(new Rect());
        }

        private void OnSelectionChange(string newKey)
        {
            this.gameDesignerEditorProfile.GameDesignerTreePickerProfile.SelectedKey = newKey;
            this.gameDesignerEditorProfile.ChangeCurrentModule((IGameDesignerModule)Activator.CreateInstance(this.Modules[newKey]));
        }


    }

}
