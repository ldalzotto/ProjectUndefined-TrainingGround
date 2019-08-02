using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTPuzzle;
using UnityEditor;
using System.Linq;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExploreTargetZone : ExploreModule
    {
        private List<TargetZoneModule> TargetZones = new List<TargetZoneModule>();
        private Dictionary<TargetZoneModule, Editor> TargetZonesEditor = new Dictionary<TargetZoneModule, Editor>();

        public override void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.DisplayObjects("Target zones : ", this.TargetZones,ref this.TargetZonesEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            this.TargetZones = GameObject.FindObjectsOfType<TargetZoneModule>().ToList();
        }
    }
}