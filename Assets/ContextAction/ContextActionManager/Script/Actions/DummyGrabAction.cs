[System.Serializable]
public class DummyGrabAction : AContextAction
{
    public override bool ComputeFinishedConditions()
    {
        return true;
    }

    public override void ExecuteAction(AContextActionInput ContextActionInput)
    {

    }

    public override void Tick(float d)
    {

    }
}
