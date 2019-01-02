using UnityEngine;

public class CAT : MonoBehaviour
{

    public AContextAction[] contextActions;

    private ContextActionWheel caw;
    // Use this for initialization
    void Start()
    {
        caw = GameObject.FindObjectOfType<ContextActionWheel>();
        caw.Init(contextActions);
    }

    private void Update()
    {
        var d = Time.deltaTime;
        caw.Tick(d);
    }
}
