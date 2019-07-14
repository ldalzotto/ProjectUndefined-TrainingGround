using UnityEngine;
using System.Collections;
using Editor_MainGameCreationWizard;
using Editor_PuzzleLevelCreationWizard;
using Editor_AdventureBaseLevelCreationWizard;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class CreateAdventureLevel : IGameDesignerModule
    {
        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            if (GUILayout.Button("CREATE LEVEL IN EDITOR"))
            {
                GameCreationWizard.InitWithSelected(nameof(AdventureBaseLevelCreationWizard));
            }
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}