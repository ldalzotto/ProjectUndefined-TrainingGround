using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdventureGame;
using UnityEditor;
using System.Linq;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExplorePOI : ExploreModule
    {
        private List<PointOfInterestType> PöintOfInterests = new List<PointOfInterestType>();
        private Dictionary<PointOfInterestType, Editor> PöintOfInterestsEditor = new Dictionary<PointOfInterestType, Editor>();

        public override void GUITick()
        {
            this.DisplayObjects("POI : ", this.PöintOfInterests, ref this.PöintOfInterestsEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            this.PöintOfInterests = GameObject.FindObjectsOfType<PointOfInterestType>().ToList();
        }
    }
}