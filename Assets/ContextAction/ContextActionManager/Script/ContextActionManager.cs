using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextActionManager : MonoBehaviour
{

    private PlayerManager PlayerManager;

    private void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
    }

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
        try
        {
            contextAction.FirstExecutionAction(contextActionInput);
            PlayerManager.OnContextActionAdded(contextAction);
            ExecutedContextActions.Add(contextAction);
            //first tick for removing at the same frame if necessary
            ProcessTick(0f, contextAction);
        }
        catch (Exception e)
        {
            Debug.LogError("An error occured while trying to execute ActionContext : " + contextAction.GetType() + " : " + e.Message, this);
            Debug.LogError(e.GetBaseException().StackTrace);
        }


    }

    IEnumerator RemoveContextAction(AContextAction contextAction)
    {
        yield return new WaitForEndOfFrame();
        ExecutedContextActions.Remove(contextAction);
        contextAction.ResetState();
    }

}

[System.Serializable]
public abstract class AContextAction : MonoBehaviour
{
    public abstract void FirstExecutionAction(AContextActionInput ContextActionInput);
    public abstract bool ComputeFinishedConditions();
    public abstract void Tick(float d);

    public void OnTick(float d)
    {
        if (!isFinished)
        {
            Tick(d);

            if (ComputeFinishedConditions())
            {
                Debug.Log("Action : " + this.name + " finished.");
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

    internal void ResetState()
    {
        isFinished = false;
        OnFinished = null;
    }
}

[System.Serializable]
public abstract class AContextActionInput
{

}
