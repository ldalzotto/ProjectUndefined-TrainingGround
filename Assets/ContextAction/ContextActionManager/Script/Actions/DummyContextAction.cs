using UnityEngine;

[System.Serializable]
public class DummyContextAction : AContextAction
{

    private float elapsedTime;

    public override bool ComputeFinishedConditions()
    {
        return elapsedTime >= 2f;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        var actionInput = ContextActionInput as DummyContextActionInput;
        elapsedTime = 0f;
        Debug.Log(Time.frameCount + actionInput.Text);
    }

    public override void Tick(float d)
    {
        elapsedTime += d;
    }
}

[System.Serializable]
public class DummyContextActionInput : AContextActionInput
{
    private string text;

    public DummyContextActionInput(string text)
    {
        this.text = text;
    }

    public string Text { get => text; }
}