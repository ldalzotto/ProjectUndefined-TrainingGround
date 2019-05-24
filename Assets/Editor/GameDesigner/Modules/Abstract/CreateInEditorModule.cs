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
                PuzzleCreationWizard.InitWithSelected(typeof(CREATION_WIZARD).Name);
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