using UnityEngine;

namespace InteractiveObjectTest
{
    public class AttractiveInteractiveObject : CoreInteractiveObject
    {
        private AttractiveObjectSystem AttractiveObjectSystem;

        public AttractiveInteractiveObject(InteractiveGameObject interactiveGameObject, InteractiveObjectInitializerData InteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.InteractiveObjectTag = new InteractiveObjectTag { IsAttractiveObject = true };

            var physicsInteractionSelectionGuard = new InteractiveObjectTagStruct { IsAttractiveObject = true };
            this.AttractiveObjectSystem = new AttractiveObjectSystem(this, physicsInteractionSelectionGuard, InteractiveObjectInitializerData.AttractiveObjectSystemDefinition,
                this.OnAttractiveSystemJustIntersected, this.OnAttractiveSystemJustNotIntersected, this.OnAttractiveSystemInterestedNothing);
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

        private void OnAttractiveSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject) { Debug.Log("AttractiveInteractiveObject : OnAttractiveSystemJustIntersected"); }
        private void OnAttractiveSystemJustNotIntersected(CoreInteractiveObject IntersectedInteractiveObject) { }
        private void OnAttractiveSystemInterestedNothing(CoreInteractiveObject IntersectedInteractiveObject) { }
    }

}
