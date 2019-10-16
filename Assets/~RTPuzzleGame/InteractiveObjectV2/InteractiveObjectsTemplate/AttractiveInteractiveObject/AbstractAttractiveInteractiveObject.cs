using RTPuzzle;
using UnityEngine;

namespace InteractiveObjectTest
{
    [SceneHandleDraw]
    public abstract class AbstractAttractiveInteractiveObject<INIT> : CoreInteractiveObject where INIT : AbstractAttractiveObjectInitializerData
    {
        protected INIT AttractiveObjectInitializerData;

        [VE_Nested]
        private AttractiveObjectSystem AttractiveObjectSystem;

        public AbstractAttractiveInteractiveObject(InteractiveGameObject interactiveGameObject, INIT InteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            interactiveGameObject.CreateLogicCollider(InteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            this.interactiveObjectTag = new InteractiveObjectTag { IsAttractiveObject = true };
            this.AttractiveObjectInitializerData = InteractiveObjectInitializerData;

            var physicsInteractionSelectionGuard = new InteractiveObjectTagStruct(isAi: 1);
            this.AttractiveObjectSystem = new AttractiveObjectSystem(this, physicsInteractionSelectionGuard, InteractiveObjectInitializerData.AttractiveObjectSystemDefinition,
                this.OnAssociatedAttractiveSystemJustIntersected, this.OnAssociatedAttractiveSystemNoMoreIntersected, this.OnAssociatedAttractiveSystemInterestedNothing);
        }


        public override void Tick(float d, float timeAttenuationFactor)
        {
            base.Tick(d, timeAttenuationFactor);

            this.AttractiveObjectSystem.Tick(d, timeAttenuationFactor);
            this.isAskingToBeDestroyed = this.isAskingToBeDestroyed || (this.AttractiveObjectSystem != null && this.AttractiveObjectSystem.IsAskingTobedestroyed);
        }

        public override void Destroy()
        {
            this.AttractiveObjectSystem.OnDestroy();
            base.Destroy();
        }

        #region Attractive Object Events
        private void OnAssociatedAttractiveSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemJustIntersected");
            var AIInteractiveObject = (AIInteractiveObjectTest)IntersectedInteractiveObject;
            AIInteractiveObject.OnOtherAttractiveObjectJustIntersected(this);
        }
        private void OnAssociatedAttractiveSystemInterestedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            //  Debug.Log("OnAssociatedAttractiveSystemInterestedNothing");
            var AIInteractiveObject = (AIInteractiveObjectTest)IntersectedInteractiveObject;
            AIInteractiveObject.OnOtherAttractiveObjectIntersectedNothing(this);
        }
        private void OnAssociatedAttractiveSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemNoMoreIntersected");
            IntersectedInteractiveObject.OnOtherAttractiveObjectNoMoreIntersected(this);
        }
        #endregion
    }

}
