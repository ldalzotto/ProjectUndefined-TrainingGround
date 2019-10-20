using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public abstract class AbstractAttractiveInteractiveObject<INIT> : CoreInteractiveObject where INIT : AbstractAttractiveObjectInitializerData
    {
        protected INIT AttractiveObjectInitializerData;

        [VE_Nested] private AttractiveObjectSystem AttractiveObjectSystem;

        public AbstractAttractiveInteractiveObject(IInteractiveGameObject interactiveGameObject, INIT InteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            interactiveGameObject.CreateLogicCollider(InteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            interactiveObjectTag = new InteractiveObjectTag {IsAttractiveObject = true};
            AttractiveObjectInitializerData = InteractiveObjectInitializerData;

            var physicsInteractionSelectionGuard = new InteractiveObjectTagStruct(isAi: 1);
            AttractiveObjectSystem = new AttractiveObjectSystem(this, physicsInteractionSelectionGuard, InteractiveObjectInitializerData.AttractiveObjectSystemDefinition,
                OnAssociatedAttractiveSystemJustIntersected, OnAssociatedAttractiveSystemNoMoreIntersected, OnAssociatedAttractiveSystemInterestedNothing);
        }


        public override void Tick(float d)
        {
            base.Tick(d);

            AttractiveObjectSystem.Tick(d);
            isAskingToBeDestroyed = isAskingToBeDestroyed || AttractiveObjectSystem != null && AttractiveObjectSystem.IsAskingTobedestroyed;
        }

        public override void Destroy()
        {
            AttractiveObjectSystem.OnDestroy();
            base.Destroy();
        }

        #region Attractive Object Events

        private void OnAssociatedAttractiveSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemJustIntersected");
            IntersectedInteractiveObject.OnOtherAttractiveObjectJustIntersected(this);
        }

        private void OnAssociatedAttractiveSystemInterestedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            //  Debug.Log("OnAssociatedAttractiveSystemInterestedNothing");
            IntersectedInteractiveObject.OnOtherAttractiveObjectIntersectedNothing(this);
        }

        private void OnAssociatedAttractiveSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemNoMoreIntersected");
            IntersectedInteractiveObject.OnOtherAttractiveObjectNoMoreIntersected(this);
        }

        #endregion
    }
}