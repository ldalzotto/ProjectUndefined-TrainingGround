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
        private List<TargetZoneObjectModule> TargetZones = new List<TargetZoneObjectModule>();
        private Dictionary<TargetZoneObjectModule, Editor> TargetZonesEditor = new Dictionary<TargetZoneObjectModule, Editor>();

        public override void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.DisplayObjects("Target zones : ", this.TargetZones,ref this.TargetZonesEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            this.TargetZones = GameObject.FindObjectsOfType<TargetZoneObjectModule>().ToList();
        }
    }
}