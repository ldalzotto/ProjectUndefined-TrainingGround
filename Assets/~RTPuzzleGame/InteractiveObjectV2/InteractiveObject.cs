﻿using CoreGame;
using RTPuzzle;
using System;
using static InteractiveObjectTest.AIMovementDefinitions;

namespace InteractiveObjectTest
{
    public abstract class CoreInteractiveObject
    {
        public InteractiveGameObject InteractiveGameObject { get; protected set; }

        public InteractiveObjectTag InteractiveObjectTag { get; protected set; }
        public BaseCutsceneController CutsceneController { get; private set; }

        public bool IsAskingToBeDestroyed { get; protected set; }
        public bool IsUpdatedInMainManager;

        public CoreInteractiveObject(InteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true)
        {
            this.IsAskingToBeDestroyed = false;
            this.IsUpdatedInMainManager = IsUpdatedInMainManager;
            InteractiveGameObject = interactiveGameObject;
            this.CutsceneController = new BaseCutsceneController(interactiveGameObject.Rigidbody, interactiveGameObject.Agent, interactiveGameObject.Animator);
        }

        public virtual void FixedTick(float d) { }
        public virtual void TickAlways(float d) { }
        public virtual void Tick(float d, float timeAttenuationFactor) { }
        public virtual void TickWhenTimeIsStopped() { }
        public virtual void AfterTicks() { }
        public virtual void LateTick(float d) { }

        public virtual void Destroy()
        {
            UnityEngine.Object.Destroy(this.InteractiveGameObject.InteractiveGameObjectParent);
        }

        #region Animation Object Events
        public virtual void OnAnimationObjectSetUnscaledSpeedMagnitude(AnimationObjectSetUnscaledSpeedMagnitudeEvent AnimationObjectSetUnscaledSpeedMagnitudeEvent) { }
        #endregion

        #region Attractive Object Events
        protected virtual void OnAssociatedAttractiveSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject) { }
        protected virtual void OnAssociatedAttractiveSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject) { }
        protected virtual void OnAssociatedAttractiveSystemInterestedNothing(CoreInteractiveObject IntersectedInteractiveObject) { }
        public virtual void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject) { }
        public virtual void OnOtherAttractiveObjectIntersectedNothing(CoreInteractiveObject OtherInteractiveObject) { }
        public virtual void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject) { }
        #endregion

        #region Disarm Object Events
        protected virtual void OnAssociatedDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject) { }
        protected virtual void OnAssciatedDisarmObjectTriggerExit(CoreInteractiveObject OtherInteractiveObject) { }
        public virtual void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject) { }
        public virtual void OnOtherDisarmobjectTriggerExit(CoreInteractiveObject OtherInteractiveObject) { }
        #endregion

        #region Sight Object Events
        protected virtual void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject) { }
        protected virtual void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject) { }
        protected virtual void OnSightObjectSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject) { }
        #endregion

        #region Level Completion Zone Events
        protected virtual void OnLevelCompletionTriggerEnterPlayer(CoreInteractiveObject IntersectedInteractiveObject) { }
        #endregion

        #region Selection Object Events
        protected virtual RTPPlayerAction ProvideSelectableObjectPlayerAction(PlayerInteractiveObject PlayerInteractiveObject) { return default; }
        #endregion

        #region AI Events
        public virtual void OnAIIsJustAttractedByAttractiveObject() { }
        public virtual void OnAIIsNoMoreAttractedByAttractiveObject() { }
        public virtual void OnAIIsJustDisarmingObject() { }
        public virtual void OnAIIsNoMoreJustDisarmingObject() { }
        public virtual void SetAIDestination(AIDestination AIDestination) { }
        public virtual void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition) { }
        public virtual void OnAIDestinationReached() { }
        #endregion
    }

    public class InteractiveObjectTag
    {
        public bool IsAttractiveObject;
        public bool IsAi;
        public bool IsObstacle;
        public bool IsPlayer;
        public bool IsLevelCompletionZone;
    }

    [System.Serializable]
    public struct InteractiveObjectTagStruct
    {
        public int IsAttractiveObject;
        public int IsAi;
        public int IsObstacle;
        public int IsPlayer;
        public int IsLevelCompletionZone;

        public InteractiveObjectTagStruct(int isAttractiveObject = -1, int isAi = -1, int isObstacle = -1, int isPlayer = -1, int isLevelCompletionZone = -1)
        {
            IsAttractiveObject = isAttractiveObject;
            IsAi = isAi;
            IsObstacle = isObstacle;
            IsPlayer = isPlayer;
            IsLevelCompletionZone = isLevelCompletionZone;
        }

        public bool Compare(InteractiveObjectTag InteractiveObjectTag)
        {
            return (this.IsAttractiveObject == -1 || this.IsAttractiveObject == Convert.ToInt32(InteractiveObjectTag.IsAttractiveObject))
                && (this.IsAi == -1 || this.IsAi == Convert.ToInt32(InteractiveObjectTag.IsAi))
                && (this.IsObstacle == -1 || this.IsObstacle == Convert.ToInt32(InteractiveObjectTag.IsObstacle))
                && (this.IsPlayer == -1 || this.IsPlayer == Convert.ToInt32(InteractiveObjectTag.IsPlayer))
                && (this.IsLevelCompletionZone == -1 || this.IsLevelCompletionZone == Convert.ToInt32(InteractiveObjectTag.IsLevelCompletionZone));
        }
    }
}
