using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextActionManager : MonoBehaviour
{

    private List<AContextAction> ExecutedContextActions = new List<AContextAction>();

    public void Tick(float d)
    {
        foreach (var contextAction in ExecutedContextActions)
        {
            ProcessTick(d, contextAction);
        }
    }

    private void ProcessTick(float d, AContextAction contextAction)
    {
        contextAction.OnTick(d);
        if (contextAction.IsFinished())
        {
            OnContextActionFinished(contextAction);
        }
    }

    #region Internal Events
    private void OnContextActionFinished(AContextAction contextAction)
    {
        StartCoroutine(RemoveContextAction(contextAction));
    }

    IEnumerator RemoveContextAction(AContextAction contextAction)
    {
        yield return new WaitForEndOfFrame();
        ExecutedContextActions.Remove(contextAction);
        contextAction.ResetState();
    }
    #endregion

    #region External Events
    public void OnAddAction(AContextAction contextAction, AContextActionInput contextActionInput)
    {
        contextAction.ContextActionInput = contextActionInput;
        contextAction.FirstExecutionAction(contextActionInput);
        ExecutedContextActions.Add(contextAction);
        //first tick for removing at the same frame if necessary
        ProcessTick(0f, contextAction);
    }
    #endregion
}

[System.Serializable]
public abstract class AContextAction : MonoBehaviour
{
    public abstract void OnStart();
    public abstract void FirstExecutionAction(AContextActionInput ContextActionInput);
    public abstract bool ComputeFinishedConditions();
    public abstract void AfterFinishedEventProcessed();
    public abstract void Tick(float d);

    private ContextActionEventManager ContextActionEventManager;

    #region Internal Dependencies
    private AContextActionInput contextActionInput;
    #endregion
    private void Start()
    {
        ContextActionEventManager = GameObject.FindObjectOfType<ContextActionEventManager>();
        OnStart();
    }

    public void OnTick(float d)
    {
        if (!isFinished)
        {
            Tick(d);

            if (ComputeFinishedConditions())
            {
                isFinished = true;
                ContextActionEventManager.OnContextActionFinished(this, contextActionInput);
                AfterFinishedEventProcessed();
            }
        }
    }

    private bool isFinished;

    public AContextActionInput ContextActionInput { get => contextActionInput; set => contextActionInput = value; }

    public bool IsFinished()
    {
        return isFinished;
    }

    internal void ResetState()
    {
        isFinished = false;
    }
}

[System.Serializable]
public abstract class AContextActionInput
{

}
