using UnityEngine;
using System.Collections;
using Editor_PuzzleGameCreationWizard;
using Editor_AIBehaviorCreationWizard;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class CreateBehavior : IGameDesignerModule
    {
        public void GUITick()
        {
            if(GUILayout.Button("CREATE IN EDITOR"))
            {
                PuzzleCreationWizard.InitWithSelected(nameof(AIBehaviorCreationWizard));
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