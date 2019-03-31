using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectInherentConfigurationData", menuName = "Configuration/PuzzleGame/AttractiveObjectConfiguration/AttractiveObjectInherentConfigurationData", order = 1)]
    public class AttractiveObjectInherentConfigurationData : ScriptableObject
    {
        public float EffectRange;
        public float EffectiveTime;
        public GameObject AttractiveObjectModelPrefab;
        public AttractiveObjectType AttractiveObjectPrefab;

        public AttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime, GameObject AttractiveObjectModelPrefab, AttractiveObjectType AttractiveObjectPrefab)
        {
            this.Init(effectRange, effectiveTime, AttractiveObjectModelPrefab, AttractiveObjectPrefab);
        }

        public void Init(float effectRange, float effectiveTime, GameObject AttractiveObjectModelPrefab, AttractiveObjectType AttractiveObjectPrefab)
        {
            EffectRange = effectRange;
            EffectiveTime = effectiveTime;
            this.AttractiveObjectModelPrefab = AttractiveObjectModelPrefab;
            this.AttractiveObjectPrefab = AttractiveObjectPrefab;
        }

        //TODO -> how to have special editors gui displayed ?
#if UNITY_EDITOR
        public void CreationWizardGUI()
        {
            this.EffectRange = EditorGUILayout.FloatField("Effect range", this.EffectRange);
            this.EffectiveTime = EditorGUILayout.FloatField("Effective time", this.EffectiveTime);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Attractive object model prefab", this.AttractiveObjectModelPrefab, typeof(GameObject), false);
            EditorGUILayout.ObjectField("Attractive object prefab", this.AttractiveObjectPrefab, typeof(AttractiveObjectType), false);
            EditorGUI.EndDisabledGroup();

        }
#endif
    }
}
