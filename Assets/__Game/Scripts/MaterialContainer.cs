using UnityEngine;

public class MaterialContainer : MonoBehaviour
{
    private static MaterialContainer instance;
    public static MaterialContainer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<MaterialContainer>();
            }
            return instance;
        }
    }

    public Material POISelectedMaterial;

    [Header("RT Puzzle Game")]
    public Material LaunchProjectileUnavailableMaterial;

}
