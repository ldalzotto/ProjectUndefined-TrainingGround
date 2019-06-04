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
        private List<NPCAIManager> NPCAIManagers = new List<NPCAIManager>();
        private Dictionary<NPCAIManager, Editor> NPCAIManagersEditor = new Dictionary<NPCAIManager, Editor>();

        public override void GUITick()
        {
            this.DisplayObjects("AI : ", this.NPCAIManagers, ref this.NPCAIManagersEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            this.NPCAIManagers = GameObject.FindObjectsOfType<NPCAIManager>().ToList();
        }
    }
}