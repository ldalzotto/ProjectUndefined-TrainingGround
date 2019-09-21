using RTPuzzle;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExploreRepelableObject : ExploreModule
    {
        private List<ObjectRepelInherentData> RepelableObjectsInherentDatas = new List<ObjectRepelInherentData>();
        private Dictionary<ObjectRepelInherentData, Editor> RepelableObjectsInherentDatasEditor = new Dictionary<ObjectRepelInherentData, Editor>();

        public override void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.DisplayObjects("Repelable objects : ",this.RepelableObjectsInherentDatas, ref this.RepelableObjectsInherentDatasEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            var foundObjectRepelType = GameObject.FindObjectsOfType<ObjectRepelModule>();
            if (foundObjectRepelType != null)
            {
                this.RepelableObjectsInherentDatas = foundObjectRepelType.ToList().ConvertAll(r => this.commonGameConfigurations.GetConfiguration<ObjectRepelConfiguration>().ConfigurationInherentData[r.ObjectRepelID]);
            }
        }
    }
}