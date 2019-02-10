
namespace RTPuzzle
{
    public abstract class RTPPlayerAction
    {
        public abstract SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId { get; }

        public abstract bool FinishedCondition();
        public abstract void FirstExecution();
        public abstract void Tick(float d);
        public abstract void GUITick();
        public abstract void GizmoTick();

    }
}

