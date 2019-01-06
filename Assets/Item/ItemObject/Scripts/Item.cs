﻿using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject ItemModel;
    public string ItemID;
    private ItemID itemID;

    private AContextAction[] contextActions;

    public AContextAction[] ContextActions { get => contextActions; }

    private void OnValidate()
    {
        itemID = (ItemID)Enum.Parse(typeof(ItemID), ItemID);
    }

    private void Start()
    {
        var childActions = GetComponentsInChildren(typeof(AContextAction));
        contextActions = new AContextAction[childActions.Length];

        for (var i = 0; i < contextActions.Length; i++)
        {
            contextActions[i] = (AContextAction)childActions[i];
        }
    }
}

[System.Serializable]
public enum ItemID
{
    DUMMY_ITEM,
    ID_CARD
}