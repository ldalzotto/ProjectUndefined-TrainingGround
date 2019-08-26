using UnityEngine;
using System.Collections;
using Editor_GameDesigner;
using Editor_InteractiveObjectExplorer;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class InteractiveObjectExplorer : IGameDesignerModule
    {
        [SerializeField]
        private InteractiveObjectExplorerModule interactiveObjectExplorerWindow;

        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.interactiveObjectExplorerWindow.OnGUI();
        }

        public void OnDisabled()
        { this.interactiveObjectExplorerWindow.OnDisabled(); }

        public void OnEnabled()
        {
            this.interactiveObjectExplorerWindow = new InteractiveObjectExplorerModule();
            this.interactiveObjectExplorerWindow.OnEnable();
        }
    }

}
