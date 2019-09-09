using AdventureGame;
using RTPuzzle;
using UnityEditor;
using UnityEngine;

public class AnimationCutsceneNodeMigration : EditorWindow
{
    [MenuItem("Migration/AnimationCutsceneNodeMigration")]
    static void Init()
    {
        AnimationCutsceneNodeMigration window = (AnimationCutsceneNodeMigration)EditorWindow.GetWindow(typeof(AnimationCutsceneNodeMigration));
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("MIGRATE"))
        {
            this.DoMigration();
        }
    }

    private void DoMigration()
    {
        var PuzzleCutsceneAnimationEdges = AssetFinder.SafeAssetFind<PuzzleCutsceneAnimationEdge>("t:" + typeof(PuzzleCutsceneAnimationEdge).Name);
        foreach (var PuzzleCutsceneAnimationEdge in PuzzleCutsceneAnimationEdges)
        {
            PuzzleCutsceneAnimationEdge.associatedAction.AnimationIdV2 = new CoreGame.ParametrizedAnimationID();
            PuzzleCutsceneAnimationEdge.associatedAction.AnimationIdV2.AnimationIdParametrized = false;
          //  PuzzleCutsceneAnimationEdge.associatedAction.AnimationIdV2.ParametrizedAnimationIDValue = PuzzleCutsceneAnimationEdge.associatedAction.AnimationId;
            EditorUtility.SetDirty(PuzzleCutsceneAnimationEdge);
        }

        var AdventureCutsceneAnimationEdges = AssetFinder.SafeAssetFind<CutsceneAnimationEdge>("t:" + typeof(CutsceneAnimationEdge).Name);
        foreach (var AdventureCutsceneAnimationEdge in AdventureCutsceneAnimationEdges)
        {
            AdventureCutsceneAnimationEdge.associatedAction.AnimationIdV2 = new CoreGame.ParametrizedAnimationID();
            AdventureCutsceneAnimationEdge.associatedAction.AnimationIdV2.AnimationIdParametrized = false;
         //   AdventureCutsceneAnimationEdge.associatedAction.AnimationIdV2.ParametrizedAnimationIDValue = AdventureCutsceneAnimationEdge.associatedAction.AnimationId;
            EditorUtility.SetDirty(AdventureCutsceneAnimationEdge);
        }

        AssetDatabase.SaveAssets();
    }
}

