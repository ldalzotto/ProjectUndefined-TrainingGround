using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
            StartCoroutine(RemoveContextAction(contextAction));
        }
    }

    public void AddAction(AContextAction contextAction, AContextActionInput contextActionInput)
    {
        contextAction.ExecuteAction(contextActionInput);
        ExecutedContextActions.Add(contextAction);
        //first tick for removing at the same frame if necessary
        ProcessTick(0f, contextAction);
    }

    IEnumerator RemoveContextAction(AContextAction contextAction)
    {
        yield return new WaitForEndOfFrame();
        ExecutedContextActions.Remove(contextAction);
    }

}

[System.Serializable]
public abstract class AContextAction : MonoBehaviour
{
    public abstract void ExecuteAction(AContextActionInput ContextActionInput);
    public abstract bool ComputeFinishedConditions();
    public abstract void Tick(float d);

    public void OnTick(float d)
    {
        if (!isFinished)
        {
            Tick(d);

            if (ComputeFinishedConditions())
            {
                isFinished = true;
                OnFinished.Invoke();
                OnFinished = null;
            }
        }
    }

    public delegate void ContextActionFinished();
    public event ContextActionFinished OnFinished;

    private bool isFinished;

    public bool IsFinished()
    {
        return isFinished;
    }

}

[System.Serializable]
public abstract class AContextActionInput
{

}
