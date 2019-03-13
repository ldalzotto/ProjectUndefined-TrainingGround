using System;
using System.Collections.Generic;
using UnityEditor;

namespace RTPuzzle
{
    [CustomEditor(typeof(AIComponents))]
    public class AIComponentsEditor : Editor
    {

        private Dictionary<string, Editor> cachedEditors = new Dictionary<string, Editor>();

        public override void OnInspectorGUI()
        {
            var aiComponents = target as AIComponents;
            EditorGUI.BeginChangeCheck();
            aiComponents.AIRandomPatrolComponent = EditorGUILayout.ObjectField("AIRandomPatrolComponent : ", aiComponents.AIRandomPatrolComponent, typeof(AIRandomPatrolComponent), false) as AIRandomPatrolComponent;
            if (aiComponents.AIRandomPatrolComponent != null)
            {
                this.DisplayNestedEditor(typeof(AIRandomPatrolComponent), aiComponents.AIRandomPatrolComponent).OnInspectorGUI();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
            }
            aiComponents.AIProjectileEscapeComponent = EditorGUILayout.ObjectField("AIProjectileEscapeComponent : ", aiComponents.AIProjectileEscapeComponent, typeof(AIProjectileEscapeComponent), false) as AIProjectileEscapeComponent;
            if (aiComponents.AIProjectileEscapeComponent != null)
            {
                this.DisplayNestedEditor(typeof(AIProjectileEscapeComponent), aiComponents.AIProjectileEscapeComponent).OnInspectorGUI();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
            }
            aiComponents.AITargetZoneComponent = EditorGUILayout.ObjectField("AITargetZoneComponent : ", aiComponents.AITargetZoneComponent, typeof(AITargetZoneComponent), false) as AITargetZoneComponent;
            if (aiComponents.AITargetZoneComponent != null)
            {
                this.DisplayNestedEditor(typeof(AITargetZoneComponent), aiComponents.AITargetZoneComponent).OnInspectorGUI();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
            }
            aiComponents.AIFearStunComponent = EditorGUILayout.ObjectField("AIFearStunComponent : ", aiComponents.AIFearStunComponent, typeof(AIFearStunComponent), false) as AIFearStunComponent;
            if (aiComponents.AIFearStunComponent != null)
            {
                this.DisplayNestedEditor(typeof(AIFearStunComponent), aiComponents.AIFearStunComponent).OnInspectorGUI();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(aiComponents);
            }
        }

        private Editor DisplayNestedEditor(Type windowType, UnityEngine.Object obj)
        {
            if (!this.cachedEditors.ContainsKey(windowType.Name))
            {
                this.cachedEditors[windowType.Name] = Editor.CreateEditor(obj);
            }
            return this.cachedEditors[windowType.Name];
        }

    }

}
