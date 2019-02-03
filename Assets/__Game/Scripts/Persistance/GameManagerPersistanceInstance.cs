using UnityEngine;

public class GameManagerPersistanceInstance : MonoBehaviour
{

    public GameObject GameManagerPersistancePrefab;

    private static GameManagerPersistanceInstance Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Instantiate(GameManagerPersistancePrefab, transform);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
