using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneCameraRotationAction : SequencedAction
    {
        [SerializeField]
        public float targetAngle;

        public CutsceneCameraRotationAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            var cutsceneActionInput = (CutsceneActionInput)ContextActionInput;
            cutsceneActionInput.CutsceneGlobalController.SetCameraTargetAngle(this.targetAngle);
        }


        public override void Tick(float d)
        {

        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.targetAngle = NodeEditorGUILayout.FloatField("Target angle : ", string.Empty, this.targetAngle);
        }
#endif
    }

}
