using CoreGame;
using UnityEditor;

[CustomEditor(typeof(TransformMoveManagerComponentV2))]
public class TransformMoveManagerComponentV2CustomEditor : Editor
{
    private FoldableArea PositionUpdateConstrainedFoldableArea;

    private void OnEnable()
    {
        this.PositionUpdateConstrainedFoldableArea = new FoldableArea(true, "Constrained Position Update", ((TransformMoveManagerComponentV2)target).IsPositionUpdateConstrained);
    }

    public override void OnInspectorGUI()
    {
        TransformMoveManagerComponentV2 TransformMoveManagerComponentV2 = (TransformMoveManagerComponentV2)target;
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(TransformMoveManagerComponentV2.SpeedMultiplicationFactor)));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(TransformMoveManagerComponentV2.RotationSpeed)));
        TransformMoveManagerComponentV2.IsPositionUpdateConstrained = this.PositionUpdateConstrainedFoldableArea.OnGUI(() =>
           {
               EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(TransformMoveManagerComponentV2.MinAngleThatAllowThePositionUpdate)));
           });
        serializedObject.ApplyModifiedProperties();
    }
}