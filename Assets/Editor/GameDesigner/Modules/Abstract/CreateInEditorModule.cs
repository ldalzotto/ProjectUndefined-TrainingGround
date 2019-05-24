using UnityEngine;
using System.Collections;
using Editor_PuzzleGameCreationWizard;

namespace Editor_GameDesigner
{
    public class CreateInEditorModule<CREATION_WIZARD> : IGameDesignerModule
    {
        public void GUITick()
        {
            if (GUILayout.Button("CREATE IN EDITOR"))
            {
                PuzzleCreationWizard.InitWithSelected(nameof(CREATION_WIZARD));
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