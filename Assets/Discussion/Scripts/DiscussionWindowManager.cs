using UnityEngine;

public class DiscussionWindowManager : MonoBehaviour
{
    #region External Dependencies
    private Canvas GameCanvas;
    #endregion

    private DicussionInputManager DicussionInputManager;
    private TextOnlyDiscussion OpenedDiscussion;
    private ChoicePopup OpenedChoicePopup;
    private DiscussionEventHandler DiscussionEventHandler;

    //scenario timeline update
    private ScenarioTimelineManager ScenarioTimelineManager;

    private void Start()
    {
        #region External Dependencies
        GameCanvas = GameObject.FindObjectOfType<Canvas>();
        var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        DicussionInputManager = new DicussionInputManager(GameInputManager);
        DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        ScenarioTimelineManager = GameObject.FindObjectOfType<ScenarioTimelineManager>();
    }

    public void Tick(float d)
    {
        if (OpenedDiscussion != null)
        {
            if (OpenedChoicePopup != null)
            {
                OpenedChoicePopup.Tick(d);
                if (DicussionInputManager.Tick())
                {
                    var selectedChoice = OpenedChoicePopup.GetSelectedDiscussionChoice();
                    OpenedDiscussion.ProcessDiscussionNodeTextEnd();
                    ScenarioTimelineManager.OnScenarioActionExecuted(new DiscussionChoiceScenarioAction(selectedChoice.Text));
                    //  OpenedDiscussion.ProcessDiscussionClose();
                }
            }
            else
            {
                OpenedDiscussion.Tick(d);
                if (!OpenedDiscussion.IsWriting())
                {
                    if (DicussionInputManager.Tick())
                    {
                        if (OpenedDiscussion.IsWaitingForCloseInput())
                        {
                            OpenedDiscussion.ProcessDiscussionNodeTextEnd();
                        }
                        else if (OpenedDiscussion.IsWaitingForContinueInput())
                        {
                            OpenedDiscussion.ProcessDiscussionContinue();
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
    public void OnDiscussionWindowAwake(DiscussionTextOnlyNode discussionNode, Transform position)
    {
        OpenedDiscussion = Instantiate(PrefabContainer.Instance.DiscussionUIPrefab, GameCanvas.transform, false);
        OpenedDiscussion.transform.localScale = Vector3.zero;
        OpenedDiscussion.InitializeDependencies();
        OpenedDiscussion.OnDiscussionWindowAwake(discussionNode, position);
    }

    public void OnDiscussionEnd()
    {
        OpenedDiscussion.ProcessDiscussionClose();
    }

    public void OnChoicePopupAwake(DiscussionChoiceNode nextDisucssionChoiceNode)
    {
        OpenedChoicePopup = Instantiate(PrefabContainer.Instance.ChoicePopupPrefab, OpenedDiscussion.transform);
        OpenedChoicePopup.OnChoicePopupAwake(nextDisucssionChoiceNode, Vector2.zero);
    }

    public void OnDiscussionWindowSleep()
    {
        OpenedDiscussion.OnDiscussionWindowSleep();
        Destroy(OpenedDiscussion.gameObject);
        OpenedDiscussion = null;

        if (OpenedChoicePopup != null)
        {
            Destroy(OpenedChoicePopup.gameObject);
            OpenedChoicePopup = null;
        }

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