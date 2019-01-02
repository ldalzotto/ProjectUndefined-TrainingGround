using UnityEngine;

public class GameManager : MonoBehaviour
{

    private ContextActionManager ContextActionManager;
    private ContextActionWheelManager ContextActionWheelManager;
    private PlayerManager PlayerManager;

    void Start()
    {
        ContextActionManager = FindObjectOfType<ContextActionManager>();
        ContextActionWheelManager = FindObjectOfType<ContextActionWheelManager>();
        PlayerManager = FindObjectOfType<PlayerManager>();
    }

    void Update()
    {
        var d = Time.deltaTime;
        ContextActionWheelManager.Tick(d);
        ContextActionManager.Tick(d);
        PlayerManager.Tick(d);
    }

    private void FixedUpdate()
    {
        var d = Time.fixedDeltaTime;
        PlayerManager.FixedTick(d);
    }

    private void LateUpdate()
    {
        var d = Time.deltaTime;
        PlayerManager.LateTick(d);
    }

    private void OnDrawGizmos()
    {
        if (PlayerManager != null)
        {
            PlayerManager.OnGizmoTick();
        }
    }
}
