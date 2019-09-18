﻿using GameConfigurationID;
using CoreGame;

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
        public CutsceneParametersName InteractiveObjectParameterName;

        public IInteractiveObjectTypeDataRetrieval Resolve(PuzzleCutsceneActionInput PuzzleCutsceneActionInput)
        {
            if (this.InteractiveObjectParametrized)
            {
                return ((InteractiveObjectType)PuzzleCutsceneActionInput.graphParameters[this.InteractiveObjectParameterName]);
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
                this.InteractiveObjectParameterName = (CutsceneParametersName)NodeEditorGUILayout.EnumField(label, string.Empty, this.InteractiveObjectParameterName);
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