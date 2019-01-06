using UnityEngine;

public class PlayerGlobalAnimationEventHandler : MonoBehaviour
{

    public delegate void EventHandling();
    public event EventHandling OnShowGivenItem;

    public void ShowGivenItem()
    {
        OnShowGivenItem.Invoke();
    }
}
