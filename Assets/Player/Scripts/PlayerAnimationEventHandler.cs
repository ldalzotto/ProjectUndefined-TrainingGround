using UnityEngine;

public class PlayerAnimationEventHandler : MonoBehaviour
{

    private PlayerGlobalAnimationEventHandler PlayerGlobalAnimationEventHandler;

    private void Start()
    {
        PlayerGlobalAnimationEventHandler = GameObject.FindObjectOfType<PlayerGlobalAnimationEventHandler>();
    }

    public void OnShowGivenItem()
    {
        PlayerGlobalAnimationEventHandler.ShowGivenItem();
    }

}
