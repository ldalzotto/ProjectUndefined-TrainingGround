namespace InteractiveObjectTest
{
    public class AttractiveInteractiveObject : CoreInteractiveObject
    {
        private AttractiveObjectSystem AttractiveObjectSystem;

        public AttractiveInteractiveObject(InteractiveGameObject interactiveGameObject, AttractiveObjectInitializerData InteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.InteractiveObjectTag = new InteractiveObjectTag { IsAttractiveObject = true };

            var physicsInteractionSelectionGuard = new InteractiveObjectTagStruct { IsAi = true };
            this.AttractiveObjectSystem = new AttractiveObjectSystem(this, physicsInteractionSelectionGuard, InteractiveObjectInitializerData.AttractiveObjectSystemDefinition,
                this.OnAssociatedAttractiveSystemJustIntersected, this.OnAssociatedAttractiveSystemNoMoreIntersected, this.OnAttractiveSystemInterestedNothing);
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            base.Tick(d, timeAttenuationFactor);

            this.AttractiveObjectSystem.Tick(d, timeAttenuationFactor);

            this.IsAskingToBeDestroyed = this.AttractiveObjectSystem.IsAskingTobedestroyed;
        }

        public override void Destroy()
        {
            this.AttractiveObjectSystem.OnDestroy();
            base.Destroy();
        }

        protected override void OnAssociatedAttractiveSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (IntersectedInteractiveObject.InteractiveObjectTag.IsAi)
            {
                var AIInteractiveObject = (AIInteractiveObject)IntersectedInteractiveObject;
                AIInteractiveObject.OnOtherAttractiveObjectJustIntersected(this);
            }
        }
        protected override void OnAssociatedAttractiveSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (IntersectedInteractiveObject.InteractiveObjectTag.IsAi)
            {
                IntersectedInteractiveObject.OnOtherAttractiveObjectNoMoreIntersected(this);
            }
        }
    }

}
