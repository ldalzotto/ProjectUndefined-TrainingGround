using RTPuzzle;
using UnityEditor;

[CustomEditor(typeof(LaunchProjectileInherentData))]
public class ProjectileInherentDataCutsomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var ProjectileInherentDataTarget = (LaunchProjectileInherentData)target;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script : ", MonoScript.FromScriptableObject(ProjectileInherentDataTarget), typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.ProjectileThrowRange)));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.TravelDistancePerSeconds)));

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Behavior : ");
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.isExploding)));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.ExplodingEffectRange)));

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.isPersistingToAttractiveObject)));

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Animation : ");
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.PreActionAnimation)));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.PostActionAnimation)));

        this.serializedObject.ApplyModifiedProperties();
    }
}