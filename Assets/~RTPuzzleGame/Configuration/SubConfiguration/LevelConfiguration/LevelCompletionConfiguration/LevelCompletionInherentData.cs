using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelCompletionInherentData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelCompletion/LevelCompletionInherentData", order = 1)]

    public class LevelCompletionInherentData : ScriptableObject
    {
        public LevelCompletionConditionConfiguration LevelCompletionConditionConfiguration;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelCompletionInherentData))]
    public class LevelCompletionInherentDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LevelCompletionInherentData myTarget = (LevelCompletionInherentData)target;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LevelCompletionConditionConfiguration"));

            if (myTarget.LevelCompletionConditionConfiguration != null)
            {
                EditorGUI.indentLevel += 1;
                Editor.CreateEditor(myTarget.LevelCompletionConditionConfiguration).OnInspectorGUI();
                EditorGUI.indentLevel -= 1;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif


}
