using UnityEngine;

public class DiscussionWindowManager : MonoBehaviour
{
    #region For TEST, TO REMOVE
    public string TextToWrite;
    #endregion

    private DicussionInputManager DicussionInputManager;
    private Discussion Discussion;
    private DiscussionEventHandler DiscussionEventHandler;

    private void Start()
    {
        #region External Dependencies
        var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        Discussion = GameObject.FindObjectOfType<Discussion>();
        DicussionInputManager = new DicussionInputManager(GameInputManager);
        DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
    }

    private void Update()
    {
        var d = Time.deltaTime;
        Discussion.Tick(d);

        if (!Discussion.IsWriting())
        {
            if (DicussionInputManager.Tick())
            {
                if (Discussion.IsWaitingForCloseInput())
                {
                    Discussion.ProcessDiscussionEnd();
                }
                else if (Discussion.IsWaitingForContinueInput())
                {
                    Discussion.ProcessDiscussionContinue();
                }
                else
                {
                    DiscussionEventHandler.OnDiscussionWindowAwake();
                }
            }
        }


    }

    private void OnGUI()
    {
        Discussion.OnGUIDraw();
    }

    #region External Events
    public void OnDiscussionWindowAwake()
    {
        Discussion.gameObject.SetActive(true);
        Discussion.OnDiscussionWindowAwake(TextToWrite);
    }
    public void OnDiscussionWindowSleep()
    {
        Discussion.OnDiscussionWindowSleep();
        Discussion.gameObject.SetActive(false);
    }
    #endregion

}

#region Discussion Input Handling
class DicussionInputManager
{
    private GameInputManager GameInputManager;

    public DicussionInputManager(GameInputManager gameInputManager)
    {
        GameInputManager = gameInputManager;
    }

    public bool Tick()
    {
        return GameInputManager.CurrentInput.ActionButtonD();
    }
}
#endregion