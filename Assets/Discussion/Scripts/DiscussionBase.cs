using System.Collections;
using UnityEngine;

public class DiscussionBase : MonoBehaviour
{

    private DiscussionWindowPositioner DiscussionWindowPositioner;
    private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

    public void InitializeDependencies()
    {
        #region External Event hanlder
        var discussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        #endregion

        var discussionAnimator = GetComponent<Animator>();

        DiscussionWindowPositioner = new DiscussionWindowPositioner(Camera.main, transform);
        DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator, discussionEventHandler);
    }

    public void Tick(float d)
    {
        DiscussionWindowPositioner.Tick();
    }


    #region External Events
    public void OnDiscussionWindowAwake(string fullTextContent, Transform anchoredDiscussionWorldTransform)
    {
        DiscussionWindowPositioner.SetTransformToFollow(anchoredDiscussionWorldTransform);
        DiscussionWindowAnimationManager.PlayEnterAnimation();
    }


    public void ProcessDiscussionEnd()
    {
        StartCoroutine(DiscussionWindowAnimationManager.PlayExitAnimation());
    }
    #endregion
}

#region
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