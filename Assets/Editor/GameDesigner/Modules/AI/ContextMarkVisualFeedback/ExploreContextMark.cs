using RTPuzzle;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExploreContextMark : ExploreModule
    {
        private List<ContextMarkVisualFeedbackInherentData> ContextMarkVisualFeedbackInherentData = new List<ContextMarkVisualFeedbackInherentData>();
        private Dictionary<ContextMarkVisualFeedbackInherentData, Editor> ContextMarkVisualFeedbackInherentEditor = new Dictionary<ContextMarkVisualFeedbackInherentData, Editor>();

        public override void GUITick()
        {
            this.DisplayObjects("Context Mark : ", this.ContextMarkVisualFeedbackInherentData, ref this.ContextMarkVisualFeedbackInherentEditor);
        }

        public override void OnEnabled()
        {
            var NPCAIManagers = GameObject.FindObjectsOfType<NPCAIManager>().ToList();
            var ContextMarkVisualFeedbackConfiguration = AssetFinder.SafeSingleAssetFind<ContextMarkVisualFeedbackConfiguration>("t:" + typeof(ContextMarkVisualFeedbackConfiguration).Name);
            this.ContextMarkVisualFeedbackInherentData = NPCAIManagers.ConvertAll(n => ContextMarkVisualFeedbackConfiguration.ConfigurationInherentData[n.AiID]);
            base.OnEnabled();
        }
    }
}