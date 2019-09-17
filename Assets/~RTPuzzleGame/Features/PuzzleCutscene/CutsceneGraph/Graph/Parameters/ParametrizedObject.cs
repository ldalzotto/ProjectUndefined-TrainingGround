using UnityEngine;
using System;
using System.Collections.Generic;
using CoreGame;

#if UNITY_EDITOR
using NodeGraph_Editor;
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class ParametrizedObject
    {
        [SerializeField]
        [CustomEnum()]
        public CutsceneParametersName ObjectParameterName;

        public ParametrizedObject(CutsceneParametersName objectParameterName)
        {
            ObjectParameterName = objectParameterName;
        }

        [NonSerialized]
        private object ObjectValue;

        public void Init(Dictionary<CutsceneParametersName, object> CutsceneGraphParameters)
        {
            this.ObjectValue = CutsceneGraphParameters[this.ObjectParameterName];
        }

        public T Get<T>()
        {
            return (T)this.ObjectValue;
        }

#if UNITY_EDITOR
        public void ActionGUI(string label)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
            this.ObjectParameterName = (CutsceneParametersName)NodeEditorGUILayout.EnumField(label, string.Empty, this.ObjectParameterName);
            EditorGUILayout.EndHorizontal();
        }
#endif
    }
}
