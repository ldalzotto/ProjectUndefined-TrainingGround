#if UNITY_EDITOR
using AdventureGame;
using CoreGame;
using RTPuzzle;
using System;
using UnityEditor;
using UnityEngine;

public class CutsceneParametersNameMigration : EditorWindow
{
    [MenuItem("Migration/CutsceneParametersNameMigration")]
    public static void ShowWindow()
    {
        var CutsceneParametersNameMigration = EditorWindow.GetWindow(typeof(CutsceneParametersNameMigration));
        CutsceneParametersNameMigration.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("MIGRATE"))
        {
            var AdventureCutsceneAnimationEdges = AssetFinder.SafeAssetFind<CutsceneAnimationEdge>("t:" + typeof(CutsceneAnimationEdge).Name);
            foreach(var AdventureCutsceneAnimationEdge in AdventureCutsceneAnimationEdges)
            {
                var so = new SerializedObject(AdventureCutsceneAnimationEdge);
                so.FindProperty("associatedAction.AnimationIdV2.InteractiveObjectParameterName")
                    .enumValueIndex += 1;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(AdventureCutsceneAnimationEdge);

            }
            AssetDatabase.SaveAssets();

            var PuzzleCutsceneAnimationEdges = AssetFinder.SafeAssetFind<PuzzleCutsceneAnimationEdge>("t:" + typeof(PuzzleCutsceneAnimationEdge).Name);
            foreach (var PuzzleCutsceneAnimationEdge in PuzzleCutsceneAnimationEdges)
            {
                var so = new SerializedObject(PuzzleCutsceneAnimationEdge);
                so.FindProperty("associatedAction.AnimationIdV2.InteractiveObjectParameterName")
                    .enumValueIndex += 1;
                so.FindProperty("associatedAction.InteractiveObjectParameterName").enumValueIndex += 1;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(PuzzleCutsceneAnimationEdge);
            }
            AssetDatabase.SaveAssets();

            var PuzzleFaceTowardsEdges = AssetFinder.SafeAssetFind<PuzzleFaceTowardsEdge>("t:" + typeof(PuzzleFaceTowardsEdge).Name);
            foreach(var PuzzleFaceTowardsEdge in PuzzleFaceTowardsEdges)
            {
                var so = new SerializedObject(PuzzleFaceTowardsEdge);
                so.FindProperty("associatedAction.RotatingInteractiveObjectParameter.InteractiveObjectParameterName")
                    .enumValueIndex += 1;
                so.FindProperty("associatedAction.TargetInteractiveObjectParameter.InteractiveObjectParameterName")
                    .enumValueIndex += 1;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(PuzzleFaceTowardsEdge);
            }
            AssetDatabase.SaveAssets();

            var FollowTransformEdges = AssetFinder.SafeAssetFind<FollowTransformEdge>("t:" + typeof(FollowTransformEdge).Name);
            foreach(var FollowTransformEdge in FollowTransformEdges)
            {
                var so = new SerializedObject(FollowTransformEdge);
                so.FindProperty("associatedAction.FollowingObject.ObjectParameterName")
                    .enumValueIndex += 1;
                so.FindProperty("associatedAction.TransformToFollow.ObjectParameterName")
                    .enumValueIndex += 1;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(FollowTransformEdge);
            }
            AssetDatabase.SaveAssets();

        }
    }
}
#endif