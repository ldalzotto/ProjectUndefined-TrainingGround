using UnityEngine;
using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ProjectileInherentData", menuName = "Configuration/PuzzleGame/ProjectileConfiguration/ProjectileInherentData", order = 1)]
    public class ProjectileInherentData : ScriptableObject
    {

        [SerializeField]
        public float ProjectileThrowRange;

        [SerializeField]
        public float TravelDistancePerSeconds;

        [SerializeField]
        public InteractiveObjectType ProjectilePrefabV2;

        [SerializeField]
        public bool isExploding;

        [SerializeField]
        public bool isPersistingToAttractiveObject;

        [SerializeField]
        public float EffectRange;
        
        [Header("Animation")]

        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;

        public void Init(float effectRange, float projectileThrowRange, float travelDistancePerSeconds, InteractiveObjectType projectilePrefab, bool isExploding, bool isPersistingToAttractiveObject)
        {
            this.EffectRange = effectRange;
            this.ProjectileThrowRange = projectileThrowRange;
            this.TravelDistancePerSeconds = travelDistancePerSeconds;
            this.ProjectilePrefabV2 = projectilePrefab;
            this.isExploding = isExploding;
            this.isPersistingToAttractiveObject = isPersistingToAttractiveObject;
        }

        #region Debug purposes
        public void SetTravelDistanceDebug(float value)
        {
            TravelDistancePerSeconds = value;
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ProjectileInherentData))]
    public class CustomProjectileInherentDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var ProjectileInherentDataTarget = (ProjectileInherentData)target;
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.ProjectilePrefabV2)));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.ProjectileThrowRange)));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.TravelDistancePerSeconds)));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Behavior : ");
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.isExploding)));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.isPersistingToAttractiveObject)));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.EffectRange)));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Animation : ");
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.PreActionAnimation)));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(ProjectileInherentDataTarget.PostActionAnimation)));

            this.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
