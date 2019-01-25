using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject ItemModel;
    public ItemID ItemID;

    private List<AContextAction> contextActions;

    public List<AContextAction> ContextActions { get => contextActions; }

    private void Start()
    {
        contextActions = ItemContextActionBuilder.BuilItemContextActions(this);
    }
}
