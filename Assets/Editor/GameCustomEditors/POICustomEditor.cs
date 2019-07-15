using AdventureGame;
using Editor_GameDesigner;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(PointOfInterestType))]
    public class POICustomEditor : AbstractGameCustomEditorWithLiveSelection<PointOfInterestType, POICustomEditorContext, PointOfInterestConfigurationModule, EditPOI>
    {

        private void OnEnable()
        {
            if (this.target != null)
            {
                var poiConfiguration = AssetFinder.SafeSingleAssetFind<PointOfInterestConfiguration>("t:" + typeof(PointOfInterestConfiguration).Name);
                this.drawModules = new List<GUIDrawModule<PointOfInterestType, POICustomEditorContext>>() {
                    new POIInteractionDistance()
                };
                this.context = new POICustomEditorContext();
                this.context.PointOfInterestInherentData = poiConfiguration.ConfigurationInherentData[((PointOfInterestType)target).PointOfInterestId];
            }
        }
    }

    public class POICustomEditorContext
    {
        public PointOfInterestInherentData PointOfInterestInherentData;
    }

    class POIInteractionDistance : GUIDrawModule<PointOfInterestType, POICustomEditorContext>
    {
        public override void SceneGUI(POICustomEditorContext context, PointOfInterestType target)
        {
            Handles.color = Color.yellow;
            var maxDistanceInteraction = context.PointOfInterestInherentData.MaxDistanceToInteract;
            Handles.Label(target.transform.position + Vector3.up * maxDistanceInteraction, nameof(context.PointOfInterestInherentData.MaxDistanceToInteract), MyEditorStyles.LabelYellow);
            Handles.DrawWireDisc(target.transform.position, Vector3.up, maxDistanceInteraction);
        }
    }
}