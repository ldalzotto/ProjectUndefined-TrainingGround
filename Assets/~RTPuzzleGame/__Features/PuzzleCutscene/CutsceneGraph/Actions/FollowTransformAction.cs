using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public class FollowTransformAction : SequencedAction
    {
        [SerializeField]
        public ParametrizedObject TransformToFollow;
        [SerializeField]
        public ParametrizedObject FollowingObject;

        [NonSerialized]
        private bool hasEnded;

        public FollowTransformAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.hasEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.hasEnded = false;
            this.TransformToFollow.Init(ContextActionInput.graphParameters);
            this.FollowingObject.Init(ContextActionInput.graphParameters);
        }

        public override void Tick(float d)
        {
            this.FollowingObject.Get<GameObject>().transform.position = this.TransformToFollow.Get<Transform>().position;
            this.FollowingObject.Get<GameObject>().transform.rotation = this.TransformToFollow.Get<Transform>().rotation;
        }

        public override void Interupt()
        {
            this.hasEnded = true;
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.TransformToFollow.ActionGUI("Transform to follow : ");
            this.FollowingObject.ActionGUI("Following objet : ");
        }
#endif
    }

}
