using UnityEngine;

public class RTPlayerManagerDataRetriever : MonoBehaviour
{

    private RTPlayerManager RTPlayerManager;

    public void Init()
    {
        RTPlayerManager = GameObject.FindObjectOfType<RTPlayerManager>();
    }

    #region Data Retrieval
    public float GetPlayerSpeedMagnitude()
    {
        return RTPlayerManager.GetPlayerSpeedMagnitude();
    }
    #endregion
}
