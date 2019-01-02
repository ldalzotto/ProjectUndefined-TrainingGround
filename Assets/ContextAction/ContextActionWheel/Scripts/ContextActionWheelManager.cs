using UnityEngine;

public class ContextActionWheelManager : MonoBehaviour
{

    [Header("TO REMOVE TEST")]
    public bool SimulateContextAction;

    private PlayerManager PlayerManager;
    private ContextActionManager ContextActionManager;

    private void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        ContextActionManager = GameObject.FindObjectOfType<ContextActionManager>();
    }

    public void Tick(float d)
    {
        #region TO DELETE FOR TEST
        if (SimulateContextAction)
        {
            SimulateContextAction = false;
            TriggerContextAction(GameObject.FindObjectOfType<DummyContextAction>());
        }
        #endregion
    }

    //TODO, the triggered POI must be passed here
    private void TriggerContextAction(AContextAction contextAction)
    {
        if (contextAction.GetType() == typeof(DummyContextAction))
        {
            var dummyInput = new DummyContextActionInput("TEST");
            PlayerManager.OnContextActionAdded(contextAction);
            ContextActionManager.AddAction(contextAction, dummyInput);
        }

    }

}
