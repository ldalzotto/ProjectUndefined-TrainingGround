using System.Collections;
using UnityEngine;

public class DiscussionBase : MonoBehaviour
{

    public const string TEXT_AREA_OBJECT_NAME = "TextArea";
    public const string DISCUSSION_WINDOW_OBJECT_NAME = "DiscussionWindow";

    public DiscussionWindowDimensionsTransitionComponent DiscussionWindowDimensionsTransitionComponent;

    private TextOnlyDiscussion TextOnlyDiscussion;

    private DiscussionWindowPositioner DiscussionWindowPositioner;
    private DiscussionWindowDimensionsTransitionManager DiscussionWindowDimensionsTransitionManager;
    private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

    private DiscussionWindowType DiscussionWindowType;

    public void InitializeDependencies()
    {
        #region External Event hanlder
        var discussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        #endregion

        var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);
        var discussionAnimator = GetComponent<Animator>();

        TextOnlyDiscussion = GetComponent<TextOnlyDiscussion>();
        DiscussionWindowPositioner = new DiscussionWindowPositioner(Camera.main, transform);
        DiscussionWindowDimensionsTransitionManager = new DiscussionWindowDimensionsTransitionManager(DiscussionWindowDimensionsTransitionComponent, (RectTransform)discussionWindowObject.transform);
        DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator, discussionEventHandler);

        TextOnlyDiscussion.InitializeDependencies();
    }

    public void Tick(float d)
    {
        DiscussionWindowDimensionsTransitionManager.Tick(d);
        DiscussionWindowPositioner.Tick();
        TextOnlyDiscussion.Tick(d);
    }

    public void OnGUIDraw()
    {
        TextOnlyDiscussion.OnGUIDraw();
    }

    #region External Events
    public void OnDiscussionWindowAwake(DiscussionWindowInput discussionWindowInput)
    {
        DiscussionWindowAnimationManager.PlayEnterAnimation();
        if (discussionWindowInput.GetType() == typeof(DiscussionTextOnlyInput))
        {
            var textOnlyDiscussionInput = (DiscussionTextOnlyInput)discussionWindowInput;
            DiscussionWindowType = DiscussionWindowType.TEXT_ONLY;
            DiscussionWindowPositioner.SetTransformToFollow(textOnlyDiscussionInput.AnchoredDiscussionWorldTransform);
            TextOnlyDiscussion.OnDiscussionWindowAwake(textOnlyDiscussionInput.TextToWrite, textOnlyDiscussionInput.AnchoredDiscussionWorldTransform);
        }
    }
    public void OnDiscussionWindowSleep()
    {
        TextOnlyDiscussion.OnDiscussionWindowSleep();
    }

    public void ProcessDiscussionEnd()
    {
        StartCoroutine(DiscussionWindowAnimationManager.PlayExitAnimation());
    }

    public void OnHeightChange(float newHeight)
    {
        DiscussionWindowDimensionsTransitionManager.OnHeightChange(newHeight);
    }
    #endregion

    public DiscussionWindowType GetDiscussionType()
    {
        return this.DiscussionWindowType;
    }

    public TextOnlyDiscussion GetTextOnlyDiscussion()
    {
        return TextOnlyDiscussion;
    }
}

#region Discussion Window Type
public enum DiscussionWindowType
{
    TEXT_ONLY, CHOICE
}
#endregion

#region Discussion Window Position
class DiscussionWindowPositioner
{
    private Camera camera;
    private Transform discussionTransform;
    private Transform worldTransformToFollow;

    public DiscussionWindowPositioner(Camera camera, Transform discussionTransform)
    {
        this.camera = camera;
        this.discussionTransform = discussionTransform;
    }

    public void SetTransformToFollow(Transform worldTransformToFollow)
    {
        this.worldTransformToFollow = worldTransformToFollow;
    }

    public void Tick()
    {
        if (worldTransformToFollow != null)
        {
            this.discussionTransform.position = camera.WorldToScreenPoint(worldTransformToFollow.position);
        }
    }
}
#endregion

#region Dimensions Transitions
[System.Serializable]
public class DiscussionWindowDimensionsTransitionComponent
{
    public float HeightTransitionsSpeed;
}

class DiscussionWindowDimensionsTransitionManager
{

    private DiscussionWindowDimensionsTransitionComponent DiscussionWindowDimensionsTransitionComponent;
    private RectTransform discussionWindowTransform;

    public DiscussionWindowDimensionsTransitionManager(DiscussionWindowDimensionsTransitionComponent discussionWindowDimensionsTransitionComponent, RectTransform discussionWindowTransform)
    {
        DiscussionWindowDimensionsTransitionComponent = discussionWindowDimensionsTransitionComponent;
        this.discussionWindowTransform = discussionWindowTransform;
    }

    private float targetWindowheight;
    private bool heightUpdating;

    public void Tick(float d)
    {
        if (heightUpdating)
        {
            discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, Mathf.Lerp(discussionWindowTransform.sizeDelta.y, targetWindowheight, d * DiscussionWindowDimensionsTransitionComponent.HeightTransitionsSpeed));
            if (Mathf.Abs(targetWindowheight - discussionWindowTransform.sizeDelta.y) <= 0.05)
            {
                discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, targetWindowheight);
                heightUpdating = false;
            }
        }
    }

    public void OnHeightChange(float newHeight)
    {
        heightUpdating = true;
        targetWindowheight = newHeight;
    }

}
#endregion

#region Discussion Window Animation
class DiscussionWindowAnimationManager
{
    private const string ENTER_ANIMATION_NAME = "DiscussionWindowEnterAnimation";
    private const string EXIT_ANIMATION_NAME = "DiscussionWindowExitAnimation";

    private Animator DiscussionAnimator;
    private DiscussionEventHandler DiscussionEventHandler;

    public DiscussionWindowAnimationManager(Animator discussionAnimator, DiscussionEventHandler discussionEventHandler)
    {
        DiscussionAnimator = discussionAnimator;
        DiscussionEventHandler = discussionEventHandler;
    }

    public void PlayEnterAnimation()
    {
        DiscussionAnimator.Play(ENTER_ANIMATION_NAME);
    }

    public IEnumerator PlayExitAnimation()
    {
        DiscussionAnimator.Play(EXIT_ANIMATION_NAME);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(DiscussionAnimator, EXIT_ANIMATION_NAME, 0);
        DiscussionEventHandler.OnDiscussionWindowSleep();
    }
}
#endregion

#region Discussion Window Inputs
public interface DiscussionWindowInput { };
public class DiscussionTextOnlyInput : DiscussionWindowInput
{
    private Transform anchoredDiscussionWorldTransform;
    private string textToWrite;

    public DiscussionTextOnlyInput(Transform anchoredDiscussionWorldTransform, string textToWrite)
    {
        this.anchoredDiscussionWorldTransform = anchoredDiscussionWorldTransform;
        this.textToWrite = textToWrite;
    }

    public Transform AnchoredDiscussionWorldTransform { get => anchoredDiscussionWorldTransform; }
    public string TextToWrite { get => textToWrite; }
}
#endregion