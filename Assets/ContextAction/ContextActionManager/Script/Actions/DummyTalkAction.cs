[System.Serializable]
public class DummyTalkAction : AContextAction
{
    public override void AfterFinishedEventProcessed()
    {

    }

    public override bool ComputeFinishedConditions()
    {
        throw new System.NotImplementedException();
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        throw new System.NotImplementedException();
    }

    public override void Tick(float d)
    {
        throw new System.NotImplementedException();
    }
}
