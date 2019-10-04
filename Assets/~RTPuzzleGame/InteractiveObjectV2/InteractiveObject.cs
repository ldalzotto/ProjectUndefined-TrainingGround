using System;

namespace InteractiveObjectTest
{
    public abstract class CoreInteractiveObject
    {
        public InteractiveGameObject InteractiveGameObject { get; protected set; }

        public InteractiveObjectTag InteractiveObjectTag { get; protected set; }

        public bool IsAskingToBeDestroyed { get; protected set; }

        public CoreInteractiveObject(InteractiveGameObject interactiveGameObject)
        {
            this.IsAskingToBeDestroyed = false;
            InteractiveGameObject = interactiveGameObject;
        }

        public virtual void TickAlways(float d) { }
        public virtual void Tick(float d, float timeAttenuationFactor) { }
        public virtual void TickWhenTimeIsStopped() { }
        public virtual void AfterTicks() { }

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
        protected virtual void OnAttractiveSystemInterestedNothing(CoreInteractiveObject IntersectedInteractiveObject) { }
        public virtual void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject) { }
        public virtual void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject) { }
        #endregion

        #region Disarm Object Events
        protected virtual void OnAssociatedDisarmObjectTriggerEnter(CoreInteractiveObject IntersectedInteractiveObject) { }
        public virtual void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject, out bool success) { success = false; }
        #endregion

        #region AI Events
        public virtual void OnAIIsJustAttractedByAttractiveObject(CoreInteractiveObject AttractedInteractiveObject) { }
        public virtual void OnAIIsNoMoreAttractedByAttractiveObject(CoreInteractiveObject AttractedInteractiveObject) { }
        public virtual void OnAIIsJustDisarmingObject() { }
        public virtual void OnAIIsNoMoreJustDisarmingObject() { }
        public virtual void SetAIDestination(AIDestination AIDestination) { }
        #endregion
    }

    public class InteractiveObjectTag
    {
        public bool IsAttractiveObject;
        public bool IsAi;
        public bool IsObstacle;
    }

    [System.Serializable]
    public struct InteractiveObjectTagStruct
    {
        public int IsAttractiveObject;
        public int IsAi;

        public InteractiveObjectTagStruct(int isAttractiveObject = -1, int isAi = -1)
        {
            IsAttractiveObject = isAttractiveObject;
            IsAi = isAi;
        }

        public bool Compare(InteractiveObjectTag InteractiveObjectTag)
        {
            return (this.IsAttractiveObject == -1 || this.IsAttractiveObject == Convert.ToInt32(InteractiveObjectTag.IsAttractiveObject))
                && (this.IsAi == -1 || this.IsAi == Convert.ToInt32(InteractiveObjectTag.IsAi));
        }
    }
}
