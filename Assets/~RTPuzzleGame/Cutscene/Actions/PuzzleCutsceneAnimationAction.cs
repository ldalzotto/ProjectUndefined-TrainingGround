using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEditor;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class PuzzleCutsceneAnimationAction : AbstractCutsceneAnimationAction
    {
        [CustomEnum()]
        public InteractiveObjectID InteractiveObjectID;

        public bool InteractiveObjectParametrized;
        [CustomEnum()]
        public PuzzleCutsceneParametersName InteractiveObjectParameterName;

        public PuzzleCutsceneAnimationAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        protected override AbstractCutsceneController GetAbstractCutsceneController(SequencedActionInput ContextActionInput)
        {
            var PuzzleCutsceneActionInput = (PuzzleCutsceneActionInput)ContextActionInput;
            if (this.InteractiveObjectParametrized)
            {
                return ((InteractiveObjectType)PuzzleCutsceneActionInput.PuzzleCutsceneGraphParameters[this.InteractiveObjectParameterName]).GetModule<InteractiveObjectCutsceneControllerModule>().InteractiveObjectCutsceneController;
            }
            else
            {
                return PuzzleCutsceneActionInput.InteractiveObjectContainer.GetInteractiveObjectFirst(this.InteractiveObjectID).GetModule<InteractiveObjectCutsceneControllerModule>().InteractiveObjectCutsceneController;
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
            this.InteractiveObjectParametrized = EditorGUILayout.Toggle(this.InteractiveObjectParametrized);
            if (this.InteractiveObjectParametrized)
            {
                this.InteractiveObjectParameterName = (PuzzleCutsceneParametersName) NodeEditorGUILayout.EnumField("InteractiveObject param : ", string.Empty, this.InteractiveObjectParameterName);
            }
            else
            {
                this.InteractiveObjectID = (InteractiveObjectID)NodeEditorGUILayout.EnumField("InteractiveObjectID : ", string.Empty, this.InteractiveObjectID);
            }
            EditorGUILayout.EndHorizontal();
            this.AnimationId = (AnimationID)NodeEditorGUILayout.EnumField("Animation : ", string.Empty, this.AnimationId);
            this.SkipToNextNode = (bool)NodeEditorGUILayout.BoolField("Skip immediately : ", string.Empty, this.SkipToNextNode);
            this.CrossFade = NodeEditorGUILayout.FloatField("Crossfade : ", string.Empty, this.CrossFade);
            this.PlayImmediately = (bool)NodeEditorGUILayout.BoolField("Update model positions on start : ", string.Empty, this.PlayImmediately);

            EditorGUI.BeginDisabledGroup(this.SkipToNextNode);
            this.InfiniteLoop = (bool)NodeEditorGUILayout.BoolField("Infinite loop : ", string.Empty, this.InfiniteLoop);
            this.FramePerfectEndDetection = (bool)NodeEditorGUILayout.BoolField("Frame perfect end detection : ", string.Empty, this.FramePerfectEndDetection);
            EditorGUI.EndDisabledGroup();
        }
#endif
    }

}
