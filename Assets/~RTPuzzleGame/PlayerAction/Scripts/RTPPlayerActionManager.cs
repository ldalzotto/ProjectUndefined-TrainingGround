using UnityEngine;

public class RTPPlayerActionManager : MonoBehaviour
{

    private RTPPlayerActionExecutionManager RTPPlayerActionExecutionManager;

    public void Init()
    {
        #region External Dependencies
        var RTPPlayerActionEventManager = GameObject.FindObjectOfType<RTPPlayerActionEventManager>();
        #endregion
        RTPPlayerActionExecutionManager = new RTPPlayerActionExecutionManager(RTPPlayerActionEventManager);
    }

    public void Tick(float d)
    {
        RTPPlayerActionExecutionManager.Tick(d);
    }


    #region External Events
    public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
    {
        RTPPlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
    }
    internal void StopAction()
    {
        RTPPlayerActionExecutionManager.StopAction();
    }
    #endregion

    #region Logical Conditions
    public bool IsActionExecuting()
    {
        return RTPPlayerActionExecutionManager.IsActionExecuting;
    }
    #endregion
}


#region Action execution
class RTPPlayerActionExecutionManager
{

    private RTPPlayerActionEventManager RTPPlayerActionEventManager;

    public RTPPlayerActionExecutionManager(RTPPlayerActionEventManager rTPPlayerActionEventManager)
    {
        RTPPlayerActionEventManager = rTPPlayerActionEventManager;
    }

    private RTPPlayerAction currentAction;
    private bool isActionExecuting;

    public bool IsActionExecuting { get => isActionExecuting; }

    public void Tick(float d)
    {
        if (currentAction != null)
        {
            if (currentAction.FinishedCondition())
            {
                RTPPlayerActionEventManager.OnRTPPlayerActionStop(currentAction);
            }
            else
            {
                currentAction.Tick(d);
            }
        }
    }

    public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
    {
        currentAction = rTPPlayerAction;
        isActionExecuting = true;
        currentAction.FirstExecution();
    }

    internal void StopAction()
    {
        currentAction = null;
        isActionExecuting = false;
    }
}
#endregion