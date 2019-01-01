using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private ContextActionManager ContextActionManager;
    private PlayerManager PlayerManager;

    void Start()
    {
        ContextActionManager = FindObjectOfType<ContextActionManager>();
        PlayerManager = FindObjectOfType<PlayerManager>();
    }

    void Update()
    {
        var d = Time.deltaTime;
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
