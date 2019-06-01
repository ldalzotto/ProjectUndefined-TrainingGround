using CoreGame;
using UnityEngine;

public class GameManagerPersistanceInstance : MonoBehaviour
{

    public GameObject GameManagerPersistancePrefab;

    private static GameManagerPersistanceInstance Instance;

    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            Instantiate(GameManagerPersistancePrefab, transform);
            GameObject.FindObjectOfType<PersistanceManager>().Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (gameObject != Instance.gameObject)
            {
                Destroy(gameObject);
            }
        }
    }

}
