using GameConfigurationID;
using CoreGame;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using NodeGraph_Editor;
#endif

namespace CoreGame
{
    [System.Serializable]
    public class ParametrizedAnimationID
    {
        [CustomEnum()]
        public AnimationID ParametrizedAnimationIDValue;
        public bool AnimationIdParametrized;
        [CustomEnum()]
        public CutsceneParametersName InteractiveObjectParameterName;

        public AnimationID Resolve(Dictionary<CutsceneParametersName, object> parameters)
        {
            if (this.AnimationIdParametrized)
            {
                return ((AnimationID)parameters[this.InteractiveObjectParameterName]);
            }
            else
            {
                return this.ParametrizedAnimationIDValue;
            }
        }

#if UNITY_EDITOR
        public void ActionGUI(string label)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
            this.AnimationIdParametrized = EditorGUILayout.Toggle(this.AnimationIdParametrized);
            if (this.AnimationIdParametrized)
            {
                this.InteractiveObjectParameterName = (CutsceneParametersName)NodeEditorGUILayout.EnumField(label, string.Empty, this.InteractiveObjectParameterName);
            }
            else
            {
                this.ParametrizedAnimationIDValue = (AnimationID)NodeEditorGUILayout.EnumField(label, string.Empty, this.ParametrizedAnimationIDValue);
            }
            EditorGUILayout.EndHorizontal();
        }
#endif

    }

}
