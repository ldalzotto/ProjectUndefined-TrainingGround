using InteractiveObjects_Interfaces;
using RTPuzzle;
using SelectableObject;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class TestAttractiveObject : AbstractAttractiveInteractiveObject<TestAttractiveObjectInitializerData>
    {
        [VE_Ignore] private RTPPlayerAction AssociatedPlayerAction;
        [VE_Nested] [DrawNested] private DisarmObjectSystem DisarmObjectSystem;

        [VE_Nested] private SelectableObjectSystem SelectableObjectSystem;

        public TestAttractiveObject(IInteractiveGameObject interactiveGameObject, TestAttractiveObjectInitializerData InteractiveObjectInitializerData) : base(interactiveGameObject, InteractiveObjectInitializerData)
        {
            DisarmObjectSystem = new DisarmObjectSystem(this, InteractiveObjectInitializerData.DisarmSystemDefinition, new InteractiveObjectTagStruct {IsAi = 1}, OnAssociatedDisarmObjectTriggerEnter, OnAssciatedDisarmObjectTriggerExit);
            SelectableObjectSystem = new SelectableObjectSystem(this, InteractiveObjectInitializerData.SelectableObjectSystemDefinition,
                AttractiveObjectInitializerData.SelectableGrabActionDefinition.BuildPlayerAction(PlayerInteractiveObjectManager.Get().PlayerInteractiveObject));
            AfterConstructor();
        }

        public override void Tick(float d)
        {
            if (DisarmObjectSystem != null) DisarmObjectSystem.Tick(d);

            base.Tick(d);

            DisarmObjectSystem.Tick(d);
            isAskingToBeDestroyed = isAskingToBeDestroyed || DisarmObjectSystem != null && DisarmObjectSystem.IsTimeElasped();
        }

        public override void Destroy()
        {
            DisarmObjectSystem.OnDestroy();
            SelectableObjectSystem.OnDestroy();
            base.Destroy();
        }

        #region Disarm Object Events

        private void OnAssociatedDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            OtherInteractiveObject.OnOtherDisarmObjectTriggerEnter(this);
            DisarmObjectSystem.AddInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
        }

        private void OnAssciatedDisarmObjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
            OtherInteractiveObject.OnOtherDisarmobjectTriggerExit(this);
            DisarmObjectSystem.RemoveInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
        }

        #endregion
    }
}