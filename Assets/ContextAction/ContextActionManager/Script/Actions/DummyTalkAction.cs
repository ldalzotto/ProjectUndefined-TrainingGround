[System.Serializable]
public class DummyTalkAction : AContextAction
{
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
