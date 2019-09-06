using AdventureGame;
using UnityEditor;
using UnityEngine;

public class PointOfInterestVisualMovementInherentDataMigration : EditorWindow
{
    [MenuItem("Migration/PointOfInterestVisualMovementInherentDataMigration")]
    static void Init()
    {
        PointOfInterestVisualMovementInherentDataMigration window = (PointOfInterestVisualMovementInherentDataMigration)EditorWindow.GetWindow(typeof(PointOfInterestVisualMovementInherentDataMigration));
        window.Show();
    }

    private PointOfInterestVisualMovementInherentData TargetPointOfInterestVisualMovementInherentData;
    private Editor TargetPointOfInterestVisualMovementInherentDataEditor;

    private void OnGUI()
    {
        this.DoTargetValue();
        if (GUILayout.Button("MIGRATE ALL"))
        {
            this.DoMigration();
        }
    }

    private void OnEnable()
    {
        this.TargetPointOfInterestVisualMovementInherentData = new PointOfInterestVisualMovementInherentData();
        this.TargetPointOfInterestVisualMovementInherentDataEditor = Editor.CreateEditor(this.TargetPointOfInterestVisualMovementInherentData);
    }

    private void DoTargetValue()
    {
        EditorGUILayout.LabelField("Target Values : ");
        if (this.TargetPointOfInterestVisualMovementInherentDataEditor == null || this.TargetPointOfInterestVisualMovementInherentData == null)
        {
            this.OnEnable();
        }

        this.TargetPointOfInterestVisualMovementInherentDataEditor.OnInspectorGUI();
        EditorGUILayout.Separator();
    }

    private void DoMigration()
    {
        var PointOfInterestVisualMovementConfiguration = AssetFinder.SafeSingleAssetFind<PointOfInterestVisualMovementConfiguration>("t:" + typeof(PointOfInterestVisualMovementConfiguration).Name);
        foreach (var PointOfInterestVisualMovementConfigurationValue in PointOfInterestVisualMovementConfiguration.ConfigurationInherentData.Values)
        {
            PointOfInterestVisualMovementConfigurationValue.MovingBone = this.TargetPointOfInterestVisualMovementInherentData.MovingBone;
            PointOfInterestVisualMovementConfigurationValue.RotationAngleLimit = this.TargetPointOfInterestVisualMovementInherentData.RotationAngleLimit;
            PointOfInterestVisualMovementConfigurationValue.SmoothMovementSpeed = this.TargetPointOfInterestVisualMovementInherentData.SmoothMovementSpeed;
            PointOfInterestVisualMovementConfigurationValue.SmoothOutMaxDotProductLimit = this.TargetPointOfInterestVisualMovementInherentData.SmoothOutMaxDotProductLimit;
            EditorUtility.SetDirty(PointOfInterestVisualMovementConfigurationValue);
        }
        AssetDatabase.SaveAssets();
    }
}
