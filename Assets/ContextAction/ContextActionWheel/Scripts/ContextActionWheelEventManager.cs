using UnityEngine;

public class ContextActionWheelEventManager : MonoBehaviour
{

    private PlayerManager PlayerManager;
    private ContextActionWheelManager ContextActionWheelManager;

    private void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        ContextActionWheelManager = GameObject.FindObjectOfType<ContextActionWheelManager>();
    }

    public void OnWheelDisabled()
    {
        PlayerManager.OnWheelDisabled();
    }

    public void OnWheelEnabled(PointOfInterestType triggeredPOI)
    {
        ContextActionWheelManager.OnAwakeWheel(triggeredPOI);
    }
}
