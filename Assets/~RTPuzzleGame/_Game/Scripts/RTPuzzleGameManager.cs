using UnityEngine;

public class RTPuzzleGameManager : MonoBehaviour
{

    public LevelZonesID PuzzleId;

    #region Persistance Dependencies
    private InventoryMenu InventoryMenu;
    #endregion

    private RTPlayerManager RTPlayerManager;
    private RTP_NPCManager RTP_NPCManager;
    private RTPPlayerActionManager RTPPlayerActionManager;

    private void Start()
    {
        InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
        InventoryMenu.gameObject.SetActive(false);

        RTPlayerManager = GameObject.FindObjectOfType<RTPlayerManager>();
        RTP_NPCManager = GameObject.FindObjectOfType<RTP_NPCManager>();
        RTPPlayerActionManager = GameObject.FindObjectOfType<RTPPlayerActionManager>();

        //Initialisations
        GameObject.FindObjectOfType<AIComponentsManager>().Init();
        GameObject.FindObjectOfType<RTPlayerManagerDataRetriever>().Init();
        RTPlayerManager.Init();
        RTP_NPCManager.Init();

        GameObject.FindObjectOfType<RTPPlayerActionEventManager>().Init();
        RTPPlayerActionManager.Init(PuzzleId);

    }

    private void Update()
    {
        var d = Time.deltaTime;
        RTPPlayerActionManager.Tick(d);
        RTPlayerManager.Tick(d);
        if (RTPlayerManager.HasPlayerMovedThisFrame())
        {
            RTP_NPCManager.EnableAgent();
            RTP_NPCManager.Tick(d);
        }
        else
        {
            RTP_NPCManager.DisableAgent();
        }

    }

    private void FixedUpdate()
    {
        var d = Time.fixedDeltaTime;
        RTPlayerManager.FixedTick(d);
    }

    private void OnDrawGizmos()
    {
        if (RTP_NPCManager != null)
        {
            RTP_NPCManager.GizmoTick();
        }

        if (RTPPlayerActionManager != null)
        {
            RTPPlayerActionManager.GizmoTick();
        }
    }

    private void OnGUI()
    {
        if (RTPPlayerActionManager != null)
        {
            RTPPlayerActionManager.GUITick();
        }
    }
}
