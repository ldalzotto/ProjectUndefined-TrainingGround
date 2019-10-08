using UnityEngine;

namespace InteractiveObjectTest
{
    public class AttractiveInteractiveObject : CoreInteractiveObject
    {

        private AttractiveObjectSystem AttractiveObjectSystem;
        private DisarmObjectSystem DisarmObjectSystem;

        public AttractiveInteractiveObject(InteractiveGameObject interactiveGameObject, AttractiveObjectInitializerData InteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.InteractiveObjectTag = new InteractiveObjectTag { IsAttractiveObject = true };

            var physicsInteractionSelectionGuard = new InteractiveObjectTagStruct(isAi: 1);
      //      this.AttractiveObjectSystem = new AttractiveObjectSystem(this, physicsInteractionSelectionGuard, InteractiveObjectInitializerData.AttractiveObjectSystemDefinition,
         //       this.OnAssociatedAttractiveSystemJustIntersected, this.OnAssociatedAttractiveSystemNoMoreIntersected, this.OnAttractiveSystemInterestedNothing);
            if (InteractiveObjectInitializerData.IsDisarmable)
            {
                this.DisarmObjectSystem = new DisarmObjectSystem(this, InteractiveObjectInitializerData.DisarmSystemDefinition, new InteractiveObjectTagStruct { IsAi = 1 }, this.OnAssociatedDisarmObjectTriggerEnter);
            }
        }

        public override void TickAlways(float d)
        {
            if (this.DisarmObjectSystem != null) { this.DisarmObjectSystem.TickAlways(d); }
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            base.Tick(d, timeAttenuationFactor);

            if (this.AttractiveObjectSystem != null)
            {
                this.AttractiveObjectSystem.Tick(d, timeAttenuationFactor);
            }
            if (this.DisarmObjectSystem != null)
            {
                this.DisarmObjectSystem.Tick(d, timeAttenuationFactor);
            }

            this.IsAskingToBeDestroyed = (this.AttractiveObjectSystem != null && this.AttractiveObjectSystem.IsAskingTobedestroyed) || (this.DisarmObjectSystem != null && this.DisarmObjectSystem.IsTimeElasped());
        }

        public override void Destroy()
        {
            if (this.AttractiveObjectSystem != null)
            {
                this.AttractiveObjectSystem.OnDestroy();
            }
            base.Destroy();
        }

        #region Attractive Object Events
        protected override void OnAssociatedAttractiveSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemJustIntersected");
            var AIInteractiveObject = (AIInteractiveObject)IntersectedInteractiveObject;
            AIInteractiveObject.OnOtherAttractiveObjectJustIntersected(this);
        }
        protected override void OnAssociatedAttractiveSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemNoMoreIntersected");
            IntersectedInteractiveObject.OnOtherAttractiveObjectNoMoreIntersected(this);
        }
        #endregion

        #region Disarm Object Events
        protected override void OnAssociatedDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            OtherInteractiveObject.OnOtherDisarmObjectTriggerEnter(this, out bool success);
            if (success)
            {
                this.DisarmObjectSystem.AddInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
            }
        }
        #endregion
    }

}
