using RTPuzzle;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class TestAttractiveObject : AbstractAttractiveInteractiveObject<TestAttractiveObjectInitializerData>
    {
        [VE_Nested]
        [DrawNested]
        private DisarmObjectSystem DisarmObjectSystem;

        [VE_Nested]
        private SelectableObjectSystem SelectableObjectSystem;

        public TestAttractiveObject(IInteractiveGameObject interactiveGameObject, TestAttractiveObjectInitializerData InteractiveObjectInitializerData) : base(interactiveGameObject, InteractiveObjectInitializerData)
        {
            this.DisarmObjectSystem = new DisarmObjectSystem(this, InteractiveObjectInitializerData.DisarmSystemDefinition, new InteractiveObjectTagStruct { IsAi = 1 }, this.OnAssociatedDisarmObjectTriggerEnter, this.OnAssciatedDisarmObjectTriggerExit);
            this.SelectableObjectSystem = new SelectableObjectSystem(this, InteractiveObjectInitializerData.SelectableObjectSystemDefinition, this.ProvideSelectableObjectPlayerAction);

            this.AfterConstructor();
        }

        public override void TickAlways(float d)
        {
            base.TickAlways(d);
            if (this.DisarmObjectSystem != null) { this.DisarmObjectSystem.TickAlways(d); }
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            base.Tick(d, timeAttenuationFactor);
            this.DisarmObjectSystem.Tick(d, timeAttenuationFactor);
            this.isAskingToBeDestroyed = this.isAskingToBeDestroyed || (this.DisarmObjectSystem != null && this.DisarmObjectSystem.IsTimeElasped());
        }

        public override void Destroy()
        {
            this.DisarmObjectSystem.OnDestroy();
            this.SelectableObjectSystem.OnDestroy();
            base.Destroy();
        }

        #region Disarm Object Events
        private void OnAssociatedDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            OtherInteractiveObject.OnOtherDisarmObjectTriggerEnter(this);
            this.DisarmObjectSystem.AddInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
        }

        private void OnAssciatedDisarmObjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
            OtherInteractiveObject.OnOtherDisarmobjectTriggerExit(this);
            this.DisarmObjectSystem.RemoveInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
        }

        #endregion

        #region Selectable Object
        private RTPPlayerAction ProvideSelectableObjectPlayerAction(PlayerInteractiveObject PlayerInteractiveObject)
        {
            return this.AttractiveObjectInitializerData.SelectableGrabActionDefinition.BuildPlayerAction(PlayerInteractiveObject);
        }
        #endregion
    }
}