using InteractiveObjects;

namespace AdventureGame
{
    public class AdventurePlayerInteractiveObject : CoreInteractiveObject
    {
        #region Systems

        [VE_Ignore] private AnimationObjectSystem AnimationObjectSystem;

        #endregion

        public AdventurePlayerInteractiveObject(IInteractiveGameObject interactiveGameObject,
            InteractiveObjectLogicCollider InteractiveObjectLogicCollider, AIAgentDefinition AIAgentDefinition, bool IsUpdatedInMainManager = true) : base(interactiveGameObject, IsUpdatedInMainManager)
        {
            interactiveObjectTag = new InteractiveObjectTag() {IsPlayer = true};
            InteractiveGameObject.CreateLogicCollider(InteractiveObjectLogicCollider);
            InteractiveGameObject.CreateAgent(AIAgentDefinition);

            AnimationObjectSystem = new AnimationObjectSystem(this);

            AfterConstructor();
        }

        public override void Tick(float d)
        {
            AnimationObjectSystem.Tick(d);
        }
    }
}