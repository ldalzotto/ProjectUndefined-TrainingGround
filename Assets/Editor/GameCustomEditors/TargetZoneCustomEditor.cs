using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using System;
using System.Collections.Generic;
using Editor_GameDesigner;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(TargetZone))]
    public class TargetZoneCustomEditor : AbstractGameCustomEditorWithLiveSelection<TargetZone, TargetZoneCustomEditorContext, TargetZonesConfiguration, EditTargetZone>
    {
        
        private void OnEnable()
        {
            if (target != null)
            {
                this.drawModules = new List<GUIDrawModule<TargetZone, TargetZoneCustomEditorContext>>()
                {
                    new AIDistanceDetection(),
                    new EscapeFOVSemiAngle()
                };

                this.context = new TargetZoneCustomEditorContext();
                var targetZone = (TargetZone)target;
                if (this.context.TargetZonesConfiguration == null)
                {
                    this.context.TargetZonesConfiguration = AssetFinder.SafeSingleAssetFind<TargetZonesConfiguration>("t:" + typeof(TargetZonesConfiguration));
                    if (this.context.TargetZonesConfiguration != null)
                    {
                        this.context.TargetZoneInherentData = this.context.TargetZonesConfiguration.ConfigurationInherentData[targetZone.TargetZoneID];
                    }
                }
            }
        }
    }

    public class TargetZoneCustomEditorContext
    {
        public TargetZonesConfiguration TargetZonesConfiguration;
        public TargetZoneInherentData TargetZoneInherentData;
    }

    internal class AIDistanceDetection : GUIDrawModule<TargetZone, TargetZoneCustomEditorContext>
    {
        public override void SceneGUI(TargetZoneCustomEditorContext context, TargetZone targetZone)
        {
            Handles.color = Color.red;
            Handles.Label(targetZone.transform.position + Vector3.up * context.TargetZoneInherentData.AIDistanceDetection, nameof(TargetZoneInherentData.AIDistanceDetection), MyEditorStyles.LabelRed);
            Handles.DrawWireDisc(targetZone.transform.position, Vector3.up, context.TargetZoneInherentData.AIDistanceDetection);
        }
    }

    class EscapeFOVSemiAngle : GUIDrawModule<TargetZone, TargetZoneCustomEditorContext>
    {
        public override void SceneGUI(TargetZoneCustomEditorContext context, TargetZone targetZone)
        {
            Handles.color = Color.yellow;
            Handles.Label(targetZone.transform.position + Vector3.up * 5f, nameof(TargetZoneInherentData.EscapeFOVSemiAngle), MyEditorStyles.LabelYellow);
            Handles.DrawWireArc(targetZone.transform.position, Vector3.up, targetZone.transform.forward, context.TargetZoneInherentData.EscapeFOVSemiAngle, 5f);
            Handles.DrawWireArc(targetZone.transform.position, Vector3.up, targetZone.transform.forward, -context.TargetZoneInherentData.EscapeFOVSemiAngle, 5f);
        }
    }



}
