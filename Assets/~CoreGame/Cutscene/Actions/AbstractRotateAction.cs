using System;
using System.Collections.Generic;
using SequencedAction;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    public abstract class AbstractRotateAction : ASequencedAction
    {
        [SerializeField] public float RotationSpeed = 1f;

        public AbstractRotateAction(Func<List<ASequencedAction>> nextActionsDeferred) : base(nextActionsDeferred)
        {
        }

        private bool finishedRotating;
        private RotateActionRequiredData RotateActionRequiredData;

        public override void FirstExecutionAction()
        {
            this.finishedRotating = false;
            this.RotateActionRequiredData = GetRotateActionRequiredData();
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

        protected abstract RotateActionRequiredData GetRotateActionRequiredData();
    }

    public struct RotateActionRequiredData
    {
        public BaseCutsceneController AbstractCutsceneController;
        public Quaternion TargetQuaternion;

        public RotateActionRequiredData(BaseCutsceneController abstractCutsceneController, Quaternion targetQuaternion)
        {
            AbstractCutsceneController = abstractCutsceneController;
            TargetQuaternion = targetQuaternion;
        }
    }
}