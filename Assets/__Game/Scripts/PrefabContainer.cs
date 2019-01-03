using UnityEngine;

public class PrefabContainer : MonoBehaviour
{

    private static PrefabContainer instance;

    public GameObject ActionWheelNodePrefab;

    public static PrefabContainer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PrefabContainer>();
            }
            return instance;
        }
    }
}
