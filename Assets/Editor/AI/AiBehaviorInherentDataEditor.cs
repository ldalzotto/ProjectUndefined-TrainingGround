using System;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{
    [CustomEditor(typeof(AIBehaviorInherentData))]
    public class AiBehaviorInherentDataEditor : Editor
    {
        private TypeSelectionerManager behaviorTypePicker = new TypeSelectionerManager();
        private Editor aiComponentsCachedEditor;
        private bool aiComponentsFoldout;

        private void OnEnable()
        {
            this.behaviorTypePicker.OnEnable(typeof(PuzzleAIBehavior<>), "Behavior : ");
        }

        public override void OnInspectorGUI()
        {
            var aiBehaviorInherentData = target as AIBehaviorInherentData;
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                this.aiComponentsCachedEditor = null; //clear cache
            }

            var changedType = this.behaviorTypePicker.OnInspectorGUI(aiBehaviorInherentData.BehaviorType);
            if (changedType != null)
            {
                aiBehaviorInherentData.BehaviorType = changedType;
            }

            if (aiBehaviorInherentData.BehaviorType != null)
            {
                GUILayout.Label(new GUIContent(this.GetAIBehavrioDescription(aiBehaviorInherentData.BehaviorType)), EditorStyles.miniLabel);
            }

            if (aiBehaviorInherentData.AIComponents != null)
            {
                EditorGUILayout.Space();
                aiComponentsFoldout = EditorGUILayout.Foldout(aiComponentsFoldout, "AI Components : ", true);
                EditorGUILayout.Space();
                if (aiComponentsCachedEditor == null)
                {
                    aiComponentsCachedEditor = Editor.CreateEditor(aiBehaviorInherentData.AIComponents);
                }
                if (aiComponentsFoldout)
                {
                    aiComponentsCachedEditor.OnInspectorGUI();
                }


            }
        }

        private string GetAIBehavrioDescription(Type aiComponentsType)
        {
            string aiComponentsDescription = string.Empty;
            AIBehaviorTypeSafeOperation.ForAllAIBehaviorType(aiComponentsType, () => { aiComponentsDescription = "The generic puzzle ai behavior."; return null; });
            return aiComponentsDescription;
        }
    }

    public class AIBehaviorTypeSafeOperation
    {

        public static void ForAllAIBehaviorType(Type componentType, Func<GenericPuzzleAIBehavior> genericPuzzleAIBehaviorOperation)
        {
            InvokeIfNotNullAndTypeCorresponds(componentType, typeof(GenericPuzzleAIBehavior), genericPuzzleAIBehaviorOperation);
        }

        private static bool InvokeIfNotNullAndTypeCorresponds(Type componentsType, Type comparedType, Func<object> action)
        {
            if (componentsType == comparedType)
            {
                if (action != null)
                {
                    action.Invoke();
                }
                return true;
            }
            return false;
        }
    }

}
