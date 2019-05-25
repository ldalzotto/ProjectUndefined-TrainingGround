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
        private List<TargetZone> TargetZones = new List<TargetZone>();
        private Dictionary<TargetZone, Editor> TargetZonesEditor = new Dictionary<TargetZone, Editor>();

        public override void GUITick()
        {
            this.DisplayObjects("Target zones : ", this.TargetZones,ref this.TargetZonesEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            this.TargetZones = GameObject.FindObjectsOfType<TargetZone>().ToList();
        }
    }
}