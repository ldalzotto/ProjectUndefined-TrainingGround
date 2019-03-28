using UnityEditor;

namespace RTPuzzle
{
    [CustomEditor(typeof(AIBehaviorInherentData))]
    public class AiBehaviorInherentDataEditor : Editor
    {
        private TypeSelectionerManager behaviorTypePicker = new TypeSelectionerManager();
        private Editor aiComponentsCachedEditor;
        private void OnEnable()
        {
            this.behaviorTypePicker.OnEnable(typeof(PuzzleAIBehavior<>), "Behavior : ");
        }

        public override void OnInspectorGUI()
        {
            var aiBehaviorInherentData = target as AIBehaviorInherentData;
            base.OnInspectorGUI();
            var changedType = this.behaviorTypePicker.OnInspectorGUI(aiBehaviorInherentData.BehaviorType);
            if (changedType != null)
            {
                aiBehaviorInherentData.BehaviorType = changedType;
            }

            if (aiBehaviorInherentData.AIComponents != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("AI Components : ", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                if (aiComponentsCachedEditor == null)
                {
                    aiComponentsCachedEditor = Editor.CreateEditor(aiBehaviorInherentData.AIComponents);
                }
                aiComponentsCachedEditor.OnInspectorGUI();

            }
        }
    }

}
