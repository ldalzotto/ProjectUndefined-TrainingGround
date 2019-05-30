using UnityEngine;
using System.Collections;
using CoreGame;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public abstract class ScenarioTimelineBaseWorkflowAction : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        public AContextActionInherentDataChain ContextActionDataChain;

#if UNITY_EDITOR
        private Editor cachedContextActionDataChainEditor;

        public override void ActionGUI()
        {
            EditorGUILayout.Separator();
            if (this.cachedContextActionDataChainEditor == null && this.ContextActionDataChain != null)
            {
                this.cachedContextActionDataChainEditor = Editor.CreateEditor(this.ContextActionDataChain);
            }
            if (this.cachedContextActionDataChainEditor != null)
            {
                this.cachedContextActionDataChainEditor.OnInspectorGUI();
            } else
            {
                this.ContextActionDataChain = (AContextActionInherentDataChain)EditorGUILayout.ObjectField(this.cachedContextActionDataChainEditor, typeof(AContextActionInherentDataChain), false);
            }
        }
#endif
    }

}
