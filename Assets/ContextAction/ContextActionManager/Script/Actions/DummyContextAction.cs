using UnityEngine;
using System.Collections;

[System.Serializable]
public class DummyContextAction : AContextAction
{
    public override bool ComputeFinishedConditions()
    {
        return true;
    }

    public override void ExecuteAction(AContextActionInput ContextActionInput)
    {
        var actionInput = ContextActionInput as DummyContextActionInput;
        Debug.Log(Time.frameCount + actionInput.Text);
    }

    public override void Tick(float d)
    {

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