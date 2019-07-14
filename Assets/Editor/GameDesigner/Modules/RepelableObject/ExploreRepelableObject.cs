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
        private List<RepelableObjectsInherentData> RepelableObjectsInherentDatas = new List<RepelableObjectsInherentData>();
        private Dictionary<RepelableObjectsInherentData, Editor> RepelableObjectsInherentDatasEditor = new Dictionary<RepelableObjectsInherentData, Editor>();

        public override void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.DisplayObjects("Repelable objects : ",this.RepelableObjectsInherentDatas, ref this.RepelableObjectsInherentDatasEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            var foundObjectRepelType = GameObject.FindObjectsOfType<ObjectRepelTypeModule>();
            if (foundObjectRepelType != null)
            {
                this.RepelableObjectsInherentDatas = foundObjectRepelType.ToList().ConvertAll(r => this.commonGameConfigurations.PuzzleGameConfigurations.RepelableObjectsConfiguration.ConfigurationInherentData[r.RepelableObjectID]);
            }
        }
    }
}