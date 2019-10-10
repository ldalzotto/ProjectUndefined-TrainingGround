using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public abstract class AbstractRotateAction : SequencedAction
    {
        [SerializeField]
        public float RotationSpeed = 1f;

        public AbstractRotateAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        private bool finishedRotating;
        private RotateActionRequiredData RotateActionRequiredData;

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.finishedRotating = false;
            this.RotateActionRequiredData = GetRotateActionRequiredData(ContextActionInput);
            this.RotateActionRequiredData.AbstractCutsceneController.AskRotation(this.RotateActionRequiredData.TargetQuaternion, this.RotationSpeed);
        }

        public override bool ComputeFinishedConditions()
        {
            return this.finishedRotating;
        }

        public override void Tick(float d)
        {
            this.RotateActionRequiredData.AbstractCutsceneController.Tick(d);
            this.finishedRotating = !this.RotateActionRequiredData.AbstractCutsceneController.IsRotating();
        }
        
        protected abstract RotateActionRequiredData GetRotateActionRequiredData(SequencedActionInput ContextActionInput);

    }

    public struct RotateActionRequiredData
    {
        public AbstractCutsceneController AbstractCutsceneController;
        public Quaternion TargetQuaternion;

        public RotateActionRequiredData(AbstractCutsceneController abstractCutsceneController, Quaternion targetQuaternion)
        {
            AbstractCutsceneController = abstractCutsceneController;
            TargetQuaternion = targetQuaternion;
        }
    }
}
