using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscussionBase : MonoBehaviour
{

    public const string TEXT_AREA_OBJECT_NAME = "TextArea";
    public const string DISCUSSION_WINDOW_OBJECT_NAME = "DiscussionWindow";

    public DiscussionWindowDimensionsTransitionComponent DiscussionWindowDimensionsTransitionComponent;
    public DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;

    private TextOnlyDiscussion TextOnlyDiscussion;
    private ChoiceDiscussion ChoiceDiscussion;

    private DiscussionWindowPositioner DiscussionWindowPositioner;
    private DiscussionWindowDimensionsManager DiscussionWindowDimensionsManager;
    private DiscussionWindowDimensionsTransitionManager DiscussionWindowDimensionsTransitionManager;
    private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

    private DiscussionWindowType DiscussionWindowType;

    //   private Canvas zanvas;

    public void InitializeDependencies()
    {

        #region External Event hanlder
        var discussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        #endregion

        var textAreaObject = gameObject.FindChildObjectRecursively(TEXT_AREA_OBJECT_NAME);
        var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);

        var discussionAnimator = GetComponent<Animator>();

        TextOnlyDiscussion = GetComponent<TextOnlyDiscussion>();
        ChoiceDiscussion = GetComponent<ChoiceDiscussion>();

        DiscussionWindowPositioner = new DiscussionWindowPositioner(Camera.main, transform);
        DiscussionWindowDimensionsManager = new DiscussionWindowDimensionsManager(DiscussionWindowDimensionsComponent, textAreaObject);
        DiscussionWindowDimensionsTransitionManager = new DiscussionWindowDimensionsTransitionManager(DiscussionWindowDimensionsTransitionComponent, (RectTransform)discussionWindowObject.transform);
        DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator, discussionEventHandler);

        TextOnlyDiscussion.InitializeDependencies(DiscussionWindowDimensionsComponent.Margin);
        ChoiceDiscussion.InitializeDependencies();
    }

    public void Tick(float d)
    {
        DiscussionWindowDimensionsTransitionManager.Tick(d);
        DiscussionWindowPositioner.Tick();

        if (DiscussionWindowType == DiscussionWindowType.TEXT_ONLY)
        {
            TextOnlyDiscussion.Tick(d);
        }
        else if (DiscussionWindowType == DiscussionWindowType.CHOICE)
        {
            ChoiceDiscussion.Tick(d);
        }

    }

    public void OnGUIDraw()
    {
        if (DiscussionWindowType == DiscussionWindowType.TEXT_ONLY)
        {
            TextOnlyDiscussion.OnGUIDraw();
        }
        else if (DiscussionWindowType == DiscussionWindowType.CHOICE)
        {
            ChoiceDiscussion.OnGUIDraw();
        }

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
        else if (discussionWindowInput.GetType() == typeof(DiscussionWindowChoiceInput))
        {
            var choiceDiscussionInput = (DiscussionWindowChoiceInput)discussionWindowInput;
            DiscussionWindowType = DiscussionWindowType.CHOICE;
            DiscussionWindowPositioner.SetTransformToFollow(choiceDiscussionInput.AnchoredDiscussionWorldTransform);
            ChoiceDiscussion.OnDiscussionWindowAwake(choiceDiscussionInput);
        }
    }
    public void OnDiscussionWindowSleep()
    {
        if (DiscussionWindowType == DiscussionWindowType.TEXT_ONLY)
        {
            TextOnlyDiscussion.OnDiscussionWindowSleep();
        }
        else if (DiscussionWindowType == DiscussionWindowType.CHOICE)
        {
            ChoiceDiscussion.OnDiscussionWindowSleep();
        }
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

    public DiscussionWindowDimensionsComputation GetDiscussionWindowDimensionsComputation()
    {
        return DiscussionWindowDimensionsManager;
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

#region DiscussionWindow Dimensions
[System.Serializable]
public class DiscussionWindowDimensionsComponent
{
    public float Margin;
}

public interface DiscussionWindowDimensionsComputation
{
    float GetCurrentWindowHeight();
    float GetMargin();
    float GetSingleLineHeight();
    float GetLineSpace();
    float GetWindowHeight(int lineNB);
}

class DiscussionWindowDimensionsManager : DiscussionWindowDimensionsComputation
{
    private DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;

    private Text textAreaText;

    public DiscussionWindowDimensionsManager(DiscussionWindowDimensionsComponent discussionWindowDimensionsComponent, GameObject textAreaObject)
    {
        DiscussionWindowDimensionsComponent = discussionWindowDimensionsComponent;

        var textAreaTransform = (RectTransform)textAreaObject.transform;
        this.textAreaText = textAreaObject.GetComponent<Text>();
        var margin = discussionWindowDimensionsComponent.Margin;
        textAreaTransform.offsetMin = new Vector2(margin, margin);
        textAreaTransform.offsetMax = new Vector2(-margin, -margin);
    }

    public float GetCurrentWindowHeight()
    {
        return textAreaText.preferredHeight + (DiscussionWindowDimensionsComponent.Margin * 2);
    }

    public float GetLineSpace()
    {
        return textAreaText.font.lineHeight;
    }

    public float GetMargin()
    {
        return DiscussionWindowDimensionsComponent.Margin;
    }

    public float GetSingleLineHeight()
    {
        return (textAreaText.font.lineHeight + textAreaText.lineSpacing);
    }

    public float GetWindowHeight(int lineNb)
    {
        return ((textAreaText.font.lineHeight + textAreaText.lineSpacing) * lineNb) + (DiscussionWindowDimensionsComponent.Margin * 2);
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
public class DiscussionWindowChoiceInput : DiscussionWindowInput
{
    private Transform anchoredDiscussionWorldTransform;
    private DiscussionChoiceIntroductionTextId introductionDiscussionId;
    private List<DiscussionChoiceTextId> choicesTextId;

    public DiscussionWindowChoiceInput(Transform anchoredDiscussionWorldTransform, DiscussionChoiceIntroductionTextId introductionDiscussionId, List<DiscussionChoiceTextId> choicesTextId)
    {
        this.anchoredDiscussionWorldTransform = anchoredDiscussionWorldTransform;
        this.introductionDiscussionId = introductionDiscussionId;
        this.choicesTextId = choicesTextId;
    }

    public Transform AnchoredDiscussionWorldTransform { get => anchoredDiscussionWorldTransform; }
    public DiscussionChoiceIntroductionTextId IntroductionDiscussionId { get => introductionDiscussionId; }
    public List<DiscussionChoiceTextId> ChoicesTextId { get => choicesTextId; }
}
#endregion