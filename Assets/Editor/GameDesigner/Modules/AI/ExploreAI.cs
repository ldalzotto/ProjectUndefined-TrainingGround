using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTPuzzle;
using UnityEditor;
using System.Linq;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExploreAI : ExploreModule
    {
        private List<AIObjectType> NPCAIManagers = new List<AIObjectType>();
        private Dictionary<AIObjectType, Editor> NPCAIManagersEditor = new Dictionary<AIObjectType, Editor>();

        public override void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.DisplayObjects("AI : ", this.NPCAIManagers, ref this.NPCAIManagersEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            this.NPCAIManagers = GameObject.FindObjectsOfType<AIObjectType>().ToList();
        }
    }
}