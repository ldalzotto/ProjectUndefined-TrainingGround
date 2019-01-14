using UnityEngine;

public class DiscussionWindowManager : MonoBehaviour
{
    #region For TEST, TO REMOVE
    public bool UpdateText;
    public string TextToWrite;

    public bool ContinueEvent;
    public bool EndEvent;
    #endregion
    private Discussion Discussion;

    private void Start()
    {
        Discussion = GameObject.FindObjectOfType<Discussion>();
    }

    private void Update()
    {
        var d = Time.deltaTime;
        Discussion.Tick(d);

        #region For TEST, TO REMOVE
        if (UpdateText)
        {
            UpdateText = false;
            Discussion.OnDiscussionWindowAwake(TextToWrite);
        }
        if (ContinueEvent)
        {
            ContinueEvent = false;
            Discussion.ProcessDiscussionContinue();
        }
        if (EndEvent)
        {
            EndEvent = false;
            Discussion.ProcessDiscussionEnd();
        }
        #endregion
    }

    private void OnGUI()
    {
        Discussion.OnGUIDraw();
    }

    #region External Events
    public void OnDiscussionWindowAwake()
    {

    }
    public void OnDiscussionWindowSleep()
    {

    }
    #endregion

}

