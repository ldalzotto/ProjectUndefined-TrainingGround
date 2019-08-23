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
        private InteractiveObjectExplorerWindow interactiveObjectExplorerWindow;

        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.interactiveObjectExplorerWindow.OnGUI();
        }

        public void OnDisabled()
        { }

        public void OnEnabled()
        {
            this.interactiveObjectExplorerWindow = new InteractiveObjectExplorerWindow();
            this.interactiveObjectExplorerWindow.OnEnable();
        }
    }

}
