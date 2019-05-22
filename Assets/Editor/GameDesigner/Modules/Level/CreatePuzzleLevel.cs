using UnityEngine;
using System.Collections;
using Editor_PuzzleGameCreationWizard;
using Editor_PuzzleLevelCreationWizard;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class CreatePuzzleLevel : IGameDesignerModule
    {
        public void GUITick()
        {
            if(GUILayout.Button("CREATE LEVEL IN EDITOR"))
            {
                PuzzleCreationWizard.InitWithSelected(nameof(PuzzleLevelCreationWizard));
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