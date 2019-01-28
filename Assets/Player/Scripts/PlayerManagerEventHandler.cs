using System.Collections;
using UnityEngine;

public class PlayerManagerEventHandler : MonoBehaviour
{

    #region External Dependencies
    private PlayerManager PlayerManager;
    #endregion

    void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
    }

    public IEnumerator OnSetDestinationCoRoutine(Vector3 destination)
    {
        return PlayerManager.SetDestinationCoRoutine(destination);
    }

}
