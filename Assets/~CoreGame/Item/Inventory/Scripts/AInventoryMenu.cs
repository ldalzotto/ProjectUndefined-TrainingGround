using UnityEngine;
using System.Collections;

public abstract class AInventoryMenu : MonoBehaviour
{
    public static AInventoryMenu FindCurrentInstance()
    {
        return Resources.FindObjectsOfTypeAll<AInventoryMenu>()[0];
    }
}
