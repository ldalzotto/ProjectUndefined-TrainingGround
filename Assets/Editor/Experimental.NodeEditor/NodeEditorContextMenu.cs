using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Experimental.Editor_NodeEditor
{
    public class NodeEditorContextMenu
    {

        private NodeEditor nodeEditorRef;

        public void Init(NodeEditor nodeEditorRef)
        {
            this.nodeEditorRef = nodeEditorRef;

        }

        private Vector2 currentmousePosition;

        public void GUITick()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                var contextMenu = new GenericMenu();
                contextMenu.AddItem(new GUIContent("Add node"), false, () =>
                {
                    this.nodeEditorRef.OnAddNode(this.currentmousePosition);
                });
                contextMenu.ShowAsContext();
            }

            this.currentmousePosition = Event.current.mousePosition;
        }

    }

}
