[System.Serializable]
public class DummyTalkAction : AContextAction
{

    public DummyTalkAction(AContextAction nextContextAction) : base(nextContextAction) { }

    public override void AfterFinishedEventProcessed()
    {

    }

    public override bool ComputeFinishedConditions()
    {
        return true;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
    }

    public override void Tick(float d)
    {
    }
}
