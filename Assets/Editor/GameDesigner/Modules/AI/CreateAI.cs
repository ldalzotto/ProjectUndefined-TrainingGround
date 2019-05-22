using UnityEngine;
using System.Collections;
using UnityEditor;
using Editor_PuzzleGameCreationWizard;
using Editor_AICreationObjectCreationWizard;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class CreateAI : IGameDesignerModule
    {
        public void GUITick()
        {
            if (GUILayout.Button("CREATE IN EDITOR"))
            {
                PuzzleCreationWizard.InitWithSelected(nameof(AIObjectCreationWizard));
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