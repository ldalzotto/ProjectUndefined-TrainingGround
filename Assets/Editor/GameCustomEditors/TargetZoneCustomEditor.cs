using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(TargetZone))]
public class TargetZoneCustomEditor : Editor
{

    public static TargetZonesConfiguration TargetZonesConfiguration;
    public static TargetZoneInherentData TargetZoneInherentData;

    private void OnEnable()
    {
        var targetZone = (TargetZone)target;
        if (TargetZonesConfiguration == null)
        {
            TargetZonesConfiguration = AssetFinder.SafeSingleAssetFind<TargetZonesConfiguration>("t:" + typeof(TargetZonesConfiguration));
            if (TargetZonesConfiguration != null)
            {
                TargetZoneInherentData = TargetZonesConfiguration.ConfigurationInherentData[targetZone.TargetZoneID];
            }
        }
    }

    private void OnSceneGUI()
    {
        var targetZone = (TargetZone)target;
        if (TargetZoneInherentData != null)
        {
            var oldColor = Handles.color;
            Handles.color = Color.red;
            Handles.Label(targetZone.transform.position + Vector3.up * TargetZoneInherentData.AIDistanceDetection, nameof(TargetZoneInherentData.AIDistanceDetection), MyEditorStyles.LabelRed);
            Handles.DrawWireDisc(targetZone.transform.position, Vector3.up, TargetZoneInherentData.AIDistanceDetection);

            Handles.color = Color.yellow;
            Handles.Label(targetZone.transform.position + Vector3.up * 5f, nameof(TargetZoneInherentData.EscapeFOVSemiAngle), MyEditorStyles.LabelYellow);
            Handles.DrawWireArc(targetZone.transform.position, Vector3.up, targetZone.transform.forward, TargetZoneInherentData.EscapeFOVSemiAngle, 5f);
            Handles.DrawWireArc(targetZone.transform.position, Vector3.up, targetZone.transform.forward, -TargetZoneInherentData.EscapeFOVSemiAngle, 5f);

            Handles.color = oldColor;
        }
    }


}
