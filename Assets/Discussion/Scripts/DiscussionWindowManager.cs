using UnityEngine;

public class DiscussionWindowManager : MonoBehaviour
{
    #region External Dependencies
    private Canvas GameCanvas;
    #endregion

    private DicussionInputManager DicussionInputManager;
    private DiscussionBase OpenedDiscussion;
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

    public void Tick(float d)
    {
        if (OpenedDiscussion != null)
        {
            OpenedDiscussion.Tick(d);
            if (OpenedDiscussion.GetDiscussionType() == DiscussionWindowType.TEXT_ONLY)
            {
                var textOnlyDiscussion = OpenedDiscussion.GetTextOnlyDiscussion();
                if (!textOnlyDiscussion.IsWriting())
                {
                    if (DicussionInputManager.Tick())
                    {
                        if (textOnlyDiscussion.IsWaitingForCloseInput())
                        {
                            OpenedDiscussion.ProcessDiscussionEnd();
                        }
                        else if (textOnlyDiscussion.IsWaitingForContinueInput())
                        {
                            textOnlyDiscussion.ProcessDiscussionContinue();
                        }
                    }
                }
            }
        }
    }

    public void GUITick()
    {
        if (OpenedDiscussion != null)
        {
            OpenedDiscussion.OnGUIDraw();
        }
    }

    #region External Events
    public void OnDiscussionWindowAwake(DiscussionWindowInput discussionWindowInput)
    {
        OpenedDiscussion = Instantiate(PrefabContainer.Instance.DiscussionUIPrefab, GameCanvas.transform, false);
        OpenedDiscussion.transform.localScale = Vector3.zero;
        OpenedDiscussion.InitializeDependencies();
        OpenedDiscussion.OnDiscussionWindowAwake(discussionWindowInput);
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