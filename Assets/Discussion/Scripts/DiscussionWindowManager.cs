using UnityEngine;

public class DiscussionWindowManager : MonoBehaviour
{
    #region For TEST, TO REMOVE
    public string TextToWrite;
    #endregion

    #region External Dependencies
    private Canvas GameCanvas;
    #endregion

    private DicussionInputManager DicussionInputManager;
    private Discussion OpenedDiscussion;
    private DiscussionEventHandler DiscussionEventHandler;

    private void Start()
    {
        #region External Dependencies
        GameCanvas = GameObject.FindObjectOfType<Canvas>();
        var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        DicussionInputManager = new DicussionInputManager(GameInputManager);
        DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
    }

    private void Update()
    {
        var d = Time.deltaTime;

        if (OpenedDiscussion != null)
        {
            OpenedDiscussion.Tick(d);
            if (!OpenedDiscussion.IsWriting())
            {
                if (DicussionInputManager.Tick())
                {
                    if (OpenedDiscussion.IsWaitingForCloseInput())
                    {
                        OpenedDiscussion.ProcessDiscussionEnd();
                    }
                    else if (OpenedDiscussion.IsWaitingForContinueInput())
                    {
                        OpenedDiscussion.ProcessDiscussionContinue();
                    }
                }
            }
        }
        else if (DicussionInputManager.Tick())
        {
            DiscussionEventHandler.OnDiscussionWindowAwake(Vector3.zero);
        }
    }

    private void OnGUI()
    {
        if (OpenedDiscussion != null)
        {
            OpenedDiscussion.OnGUIDraw();
        }
    }

    #region External Events
    public void OnDiscussionWindowAwake(Vector3 uiPosition)
    {
        OpenedDiscussion = Instantiate(PrefabContainer.Instance.DiscussionUIPrefab, GameCanvas.transform, false);
        OpenedDiscussion.transform.position = uiPosition;
        OpenedDiscussion.transform.localScale = Vector3.zero;
        OpenedDiscussion.InitializeDependencies();
        OpenedDiscussion.OnDiscussionWindowAwake(TextToWrite);
    }
    public void OnDiscussionWindowSleep()
    {
        OpenedDiscussion.OnDiscussionWindowSleep();
        Destroy(OpenedDiscussion.gameObject);
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