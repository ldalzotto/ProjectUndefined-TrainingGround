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
    [CustomEditor(typeof(TargetZoneObjectModule))]
    public class TargetZoneCustomEditor : AbstractGameCustomEditorWithLiveSelection<TargetZoneObjectModule, TargetZoneCustomEditorContext, TargetZonesConfiguration, EditTargetZone>
    {
        
        private void OnEnable()
        {
            if (target != null)
            {
                this.drawModules = new List<GUIDrawModule<TargetZoneObjectModule, TargetZoneCustomEditorContext>>()
                {
                    new AIDistanceDetection(),
                    new EscapeFOVSemiAngle(),
                    new TriggerZone()
                };

                this.context = new TargetZoneCustomEditorContext();
                var targetZone = (TargetZoneObjectModule)target;
                if (this.context.TargetZonesConfiguration == null)
                {
                    this.context.TargetZonesConfiguration = AssetFinder.SafeSingleAssetFind<TargetZonesConfiguration>("t:" + typeof(TargetZonesConfiguration));
                    if (this.context.TargetZonesConfiguration != null)
                    {
                        this.context.TargetZoneInherentData = this.context.TargetZonesConfiguration.ConfigurationInherentData[targetZone.TargetZoneID];
                    }
                }
                this.context.TargetZoneTriggerType = targetZone.transform.parent.GetComponentInChildren<LevelCompletionTriggerModule>();
            }
        }
    }

    public class TargetZoneCustomEditorContext
    {
        public TargetZonesConfiguration TargetZonesConfiguration;
        public TargetZoneInherentData TargetZoneInherentData;
        public LevelCompletionTriggerModule TargetZoneTriggerType; 
    }

    internal class AIDistanceDetection : GUIDrawModule<TargetZoneObjectModule, TargetZoneCustomEditorContext>
    {
        public override void SceneGUI(TargetZoneCustomEditorContext context, TargetZoneObjectModule targetZone)
        {
            Handles.color = Color.red;
            Handles.Label(targetZone.transform.position + Vector3.up * context.TargetZoneInherentData.AIDistanceDetection, nameof(TargetZoneInherentData.AIDistanceDetection), MyEditorStyles.LabelRed);
            Handles.DrawWireDisc(targetZone.transform.position, Vector3.up, context.TargetZoneInherentData.AIDistanceDetection);
        }
    }

    class EscapeFOVSemiAngle : GUIDrawModule<TargetZoneObjectModule, TargetZoneCustomEditorContext>
    {
        public override void SceneGUI(TargetZoneCustomEditorContext context, TargetZoneObjectModule targetZone)
        {
            Handles.color = Color.yellow;
            Handles.Label(targetZone.transform.position + Vector3.up * 5f, nameof(TargetZoneInherentData.EscapeFOVSemiAngle), MyEditorStyles.LabelYellow);
            Handles.DrawWireArc(targetZone.transform.position, Vector3.up, targetZone.transform.forward, context.TargetZoneInherentData.EscapeFOVSemiAngle, 5f);
            Handles.DrawWireArc(targetZone.transform.position, Vector3.up, targetZone.transform.forward, -context.TargetZoneInherentData.EscapeFOVSemiAngle, 5f);
        }
    }

    class TriggerZone : GUIDrawModule<TargetZoneObjectModule, TargetZoneCustomEditorContext>
    {
        public override void SceneGUI(TargetZoneCustomEditorContext context, TargetZoneObjectModule target)
        {
            HandlesHelper.DrawBoxCollider(context.TargetZoneTriggerType.GetComponentInChildren<BoxCollider>(), target.transform, Color.blue, this.GetType().Name, MyEditorStyles.LabelBlue);
        }
    }



}
