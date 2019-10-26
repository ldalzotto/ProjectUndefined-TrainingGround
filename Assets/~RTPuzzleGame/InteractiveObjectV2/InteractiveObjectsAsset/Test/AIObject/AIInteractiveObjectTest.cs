using AIObjects;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;
using VisualFeedback;

namespace InteractiveObjects
{
    public class AIInteractiveObjectTest : A_AIInteractiveObject<AIInteractiveObjectTestInitializerData>
    {
        private AIPatrolSystem AIPatrolSystem;
        private LocalCutscenePlayerSystem LocalCutscenePlayerSystem;
        [VE_Nested] private SightObjectSystem SightObjectSystem;

        public AIInteractiveObjectTest(IInteractiveGameObject interactiveGameObject, AIInteractiveObjectTestInitializerData AIInteractiveObjectInitializerData) : base(interactiveGameObject, AIInteractiveObjectInitializerData)
        {
            AIPatrollingState = new AIPatrollingState();
            AIAttractiveObjectState = new AIAttractiveObjectState(new BoolVariable(false, OnAIIsJustAttractedByAttractiveObject, OnAIIsNoMoreAttractedByAttractiveObject));
            AIDisarmObjectState = new AIDisarmObjectState(new BoolVariable(false, OnAIIsJustDisarmingObject, OnAIIsNoMoreJustDisarmingObject));
            interactiveObjectTag = new InteractiveObjectTag {IsAi = true};

            AIPatrolSystem = new AIPatrolSystem(this, AIInteractiveObjectInitializerData.AIPatrolSystemDefinition);

            SightObjectSystem = new SightObjectSystem(this, AIInteractiveObjectInitializerData.SightObjectSystemDefinition, new InteractiveObjectTagStruct {IsAttractiveObject = 1},
                OnSightObjectSystemJustIntersected, OnSightObjectSystemIntersectedNothing, OnSightObjectSystemNoMoreIntersected);
            LocalCutscenePlayerSystem = new LocalCutscenePlayerSystem();

            AfterConstructor();
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            LocalCutscenePlayerSystem.Tick(d);
            if (!AIDisarmObjectState.IsDisarming.GetValue() && !AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue()) AIPatrollingState.isPatrolling = true;

            if (AIPatrollingState.isPatrolling) AIPatrolSystem.Tick(d);

            AIMoveToDestinationSystem.Tick(d);
        }

        public override void AfterTicks(float d)
        {
            base.AfterTicks(d);
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public override void OnAIDestinationReached()
        {
            AIPatrolSystem.OnAIDestinationReached();
        }

        public override void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            AIMoveToDestinationSystem.ClearPath();
            AIDisarmObjectState.IsDisarming.SetValue(true);
        }

        public override void OnOtherDisarmobjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
            AIDisarmObjectState.IsDisarming.SetValue(false);
        }

        public void OnAIIsJustDisarmingObject()
        {
            LocalCutscenePlayerSystem.PlayCutscene(AIInteractiveObjectInitializerData.DisarmObjectAnimation.GetSequencedActions(this));
        }

        public void OnAIIsNoMoreJustDisarmingObject()
        {
            LocalCutscenePlayerSystem.KillCurrentCutscene();
        }

        private void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (!AIDisarmObjectState.IsDisarming.GetValue())
            {
                AIPatrollingState.isPatrolling = false;
                SwitchToAttractedState(IntersectedInteractiveObject);
                if (!AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue()) LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, IntersectedInteractiveObject);
            }
        }

        private void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            OnSightObjectSystemJustIntersected(IntersectedInteractiveObject);
        }

        private void OnSightObjectSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
        }

        #region State

        [VE_Nested] private AIPatrollingState AIPatrollingState;
        [VE_Nested] private AIAttractiveObjectState AIAttractiveObjectState;
        [VE_Nested] private AIDisarmObjectState AIDisarmObjectState;

        #endregion

        #region Attractive Object

        public override void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!AIDisarmObjectState.IsDisarming.GetValue()) SwitchToAttractedState(OtherInteractiveObject);
        }

        private void SwitchToAttractedState(CoreInteractiveObject OtherInteractiveObject)
        {
            if (OtherInteractiveObject.InteractiveObjectTag.IsAttractiveObject || OtherInteractiveObject.InteractiveObjectTag.IsPlayer)
            {
                AIAttractiveObjectState.SetIsAttractedByAttractiveObject(true, OtherInteractiveObject);
                AIPatrollingState.isPatrolling = false;
                SetAIDestination(new AIDestination {WorldPosition = OtherInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition});
            }
        }

        public override void OnOtherAttractiveObjectIntersectedNothing(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!AIDisarmObjectState.IsDisarming.GetValue() && !AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue()) SwitchToAttractedState(OtherInteractiveObject);
        }

        public override void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue())
            {
                AIMoveToDestinationSystem.ClearPath();
                AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            }

            LineVisualFeedbackSystem.DestroyLine(OtherInteractiveObject);
        }

        public void OnAIIsJustAttractedByAttractiveObject()
        {
            SetAISpeedAttenuationFactor(AIInteractiveObjectInitializerData.AISpeedWhenAttracted);
            LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, AIAttractiveObjectState.AttractedInteractiveObject);
        }

        public void OnAIIsNoMoreAttractedByAttractiveObject()
        {
            LineVisualFeedbackSystem.DestroyLine(AIAttractiveObjectState.AttractedInteractiveObject);
        }

        #endregion
    }
}