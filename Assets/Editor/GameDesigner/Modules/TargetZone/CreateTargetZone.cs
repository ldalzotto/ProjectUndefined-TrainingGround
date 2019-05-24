using UnityEngine;
using System.Collections;
namespace Editor_GameDesigner
{
    [System.Serializable]
    public class CreateTargetZone : IGameDesignerModule
    {
        public void GUITick()
        {
            if (GUILayout.Button("CREATE TARGET ZONE IN EDITOR"))
            {
                //PuzzleCreationWizard.InitWithSelected(nameof(PuzzleLevelCreationWizard));
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