using UnityEngine;

public class PlayerGlobalAnimationEventHandler : MonoBehaviour
{

    public delegate void EventHandling();
    public event EventHandling OnShowGivenItem;
    public event EventHandling OnHideGivenItem;
    public event EventHandling OnIdleOverideTriggerSmokeEffect;

    public void ShowGivenItem()
    {
        OnShowGivenItem.Invoke();
    }

    public void HideGivenItem()
    {
        OnHideGivenItem.Invoke();
    }

    //idle override events
    public void IdleOverideTriggerSmokeEffect()
    {
        OnIdleOverideTriggerSmokeEffect.Invoke();
    }

}
