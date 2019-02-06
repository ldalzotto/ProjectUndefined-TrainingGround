using UnityEngine;

public class RTPuzzleGameManager : MonoBehaviour
{

    #region Persistance Dependencies
    private InventoryMenu InventoryMenu;
    #endregion

    private RTPlayerManager RTPlayerManager;

    private void Start()
    {
        InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
        InventoryMenu.gameObject.SetActive(false);

        RTPlayerManager = GameObject.FindObjectOfType<RTPlayerManager>();
        RTPlayerManager.Init();
    }

    private void Update()
    {
        var d = Time.deltaTime;
        RTPlayerManager.Tick(d);
    }

    private void FixedUpdate()
    {
        var d = Time.fixedDeltaTime;
        RTPlayerManager.FixedTick(d);
    }
}
