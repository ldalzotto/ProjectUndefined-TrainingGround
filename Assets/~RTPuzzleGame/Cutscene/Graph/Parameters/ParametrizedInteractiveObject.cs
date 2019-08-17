using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
using NodeGraph_Editor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class ParametrizedInteractiveObject
    {
        [CustomEnum()]
        public InteractiveObjectID InteractiveObjectID;
        public bool InteractiveObjectParametrized;
        [CustomEnum()]
        public PuzzleCutsceneParametersName InteractiveObjectParameterName;

        public InteractiveObjectType Resolve(PuzzleCutsceneActionInput PuzzleCutsceneActionInput)
        {
            if (this.InteractiveObjectParametrized)
            {
                return ((InteractiveObjectType)PuzzleCutsceneActionInput.PuzzleCutsceneGraphParameters[this.InteractiveObjectParameterName]);
            }
            else
            {
                return PuzzleCutsceneActionInput.InteractiveObjectContainer.GetInteractiveObjectFirst(this.InteractiveObjectID);
            }
        }

#if UNITY_EDITOR
        public void ActionGUI(string label)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
            this.InteractiveObjectParametrized = EditorGUILayout.Toggle(this.InteractiveObjectParametrized);
            if (this.InteractiveObjectParametrized)
            {
                this.InteractiveObjectParameterName = (PuzzleCutsceneParametersName)NodeEditorGUILayout.EnumField(label, string.Empty, this.InteractiveObjectParameterName);
            }
            else
            {
                this.InteractiveObjectID = (InteractiveObjectID)NodeEditorGUILayout.EnumField(label, string.Empty, this.InteractiveObjectID);
            }
            EditorGUILayout.EndHorizontal();
        }
#endif

    }

}
