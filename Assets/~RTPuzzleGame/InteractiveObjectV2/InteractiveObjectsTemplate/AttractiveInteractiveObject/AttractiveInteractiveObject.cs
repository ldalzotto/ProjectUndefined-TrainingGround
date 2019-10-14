using RTPuzzle;
using UnityEngine;

namespace InteractiveObjectTest
{
    [SceneHandleDraw]
    public class AttractiveInteractiveObject : CoreInteractiveObject
    {
        private AttractiveObjectInitializerData AttractiveObjectInitializerData;

        [VE_Nested]
        private AttractiveObjectSystem AttractiveObjectSystem;

        [VE_Nested]
        [DrawNested]
        private DisarmObjectSystem DisarmObjectSystem;

        [VE_Nested]
        private SelectableObjectSystem SelectableObjectSystem;
        
        public AttractiveInteractiveObject(InteractiveGameObject interactiveGameObject, AttractiveObjectInitializerData InteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.interactiveObjectTag = new InteractiveObjectTag { IsAttractiveObject = true };
            this.AttractiveObjectInitializerData = InteractiveObjectInitializerData;

            var physicsInteractionSelectionGuard = new InteractiveObjectTagStruct(isAi: 1);
            this.AttractiveObjectSystem = new AttractiveObjectSystem(this, physicsInteractionSelectionGuard, InteractiveObjectInitializerData.AttractiveObjectSystemDefinition,
                this.OnAssociatedAttractiveSystemJustIntersected, this.OnAssociatedAttractiveSystemNoMoreIntersected, this.OnAssociatedAttractiveSystemInterestedNothing);
            if (InteractiveObjectInitializerData.IsDisarmable)
            {
                this.DisarmObjectSystem = new DisarmObjectSystem(this, InteractiveObjectInitializerData.DisarmSystemDefinition, new InteractiveObjectTagStruct { IsAi = 1 }, this.OnAssociatedDisarmObjectTriggerEnter, this.OnAssciatedDisarmObjectTriggerExit);
            }
            this.SelectableObjectSystem = new SelectableObjectSystem(this, InteractiveObjectInitializerData.SelectableObjectSystemDefinition, this.ProvideSelectableObjectPlayerAction);
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

            this.isAskingToBeDestroyed = (this.AttractiveObjectSystem != null && this.AttractiveObjectSystem.IsAskingTobedestroyed) || (this.DisarmObjectSystem != null && this.DisarmObjectSystem.IsTimeElasped());
        }

        public override void Destroy()
        {
            if (this.AttractiveObjectSystem != null)
            {
                this.AttractiveObjectSystem.OnDestroy();
            }
            this.DisarmObjectSystem.OnDestroy();
            this.SelectableObjectSystem.OnDestroy();
            base.Destroy();
        }

        #region Attractive Object Events
        protected override void OnAssociatedAttractiveSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemJustIntersected");
            var AIInteractiveObject = (AIInteractiveObjectTest)IntersectedInteractiveObject;
            AIInteractiveObject.OnOtherAttractiveObjectJustIntersected(this);
        }
        protected override void OnAssociatedAttractiveSystemInterestedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
          //  Debug.Log("OnAssociatedAttractiveSystemInterestedNothing");
            var AIInteractiveObject = (AIInteractiveObjectTest)IntersectedInteractiveObject;
            AIInteractiveObject.OnOtherAttractiveObjectIntersectedNothing(this);
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
            OtherInteractiveObject.OnOtherDisarmObjectTriggerEnter(this);
            this.DisarmObjectSystem.AddInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
        }

        protected override void OnAssciatedDisarmObjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
            OtherInteractiveObject.OnOtherDisarmobjectTriggerExit(this);
            this.DisarmObjectSystem.RemoveInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
        }

        #endregion

        #region Selectable Object
        protected override RTPPlayerAction ProvideSelectableObjectPlayerAction(PlayerInteractiveObject PlayerInteractiveObject)
        {
            return this.AttractiveObjectInitializerData.SelectableGrabActionDefinition.BuildPlayerAction(PlayerInteractiveObject);
        }
        #endregion
    }

}
