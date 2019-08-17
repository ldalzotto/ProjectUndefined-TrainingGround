using CoreGame;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class PuzzleFaceTowardsAction : AbstractRotateAction
    {
        [SerializeField]
        public ParametrizedInteractiveObject RotatingInteractiveObjectParameter;
        [SerializeField]
        public ParametrizedInteractiveObject TargetInteractiveObjectParameter;

        public PuzzleFaceTowardsAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed() { }

        protected override RotateActionRequiredData GetRotateActionRequiredData(SequencedActionInput ContextActionInput)
        {
            var rotatingObject = this.RotatingInteractiveObjectParameter.Resolve((PuzzleCutsceneActionInput)ContextActionInput);
            var targetObject = this.TargetInteractiveObjectParameter.Resolve((PuzzleCutsceneActionInput)ContextActionInput);
            Quaternion targetQuaterion = Quaternion.LookRotation(targetObject.transform.position - rotatingObject.transform.position, rotatingObject.transform.up);
            targetQuaterion.eulerAngles = targetQuaterion.eulerAngles.Mul(rotatingObject.transform.up);
            return new RotateActionRequiredData(rotatingObject.GetModule<InteractiveObjectCutsceneControllerModule>().InteractiveObjectCutsceneController, targetQuaterion);
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.RotatingInteractiveObjectParameter.ActionGUI("Object rotating : ");
            this.TargetInteractiveObjectParameter.ActionGUI("Facing to : ");
            this.RotationSpeed =  NodeEditorGUILayout.FloatField("Rotation speed : ", string.Empty, this.RotationSpeed);
        }
#endif
    }

}
