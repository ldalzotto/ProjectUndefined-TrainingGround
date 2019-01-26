﻿
using System;
using System.Collections.Generic;
using UnityEngine;

#region Point Of Interest Configuraiton
public enum PointOfInterestId
{
    NONE = 0,
    BOUNCER = 1,
    ID_CARD = 2,
    ID_CARD_V2 = 3,
    PLAYER = 4,
    DUMBSTER = 5
}
#endregion

#region Item Configuration
[System.Serializable]
public enum ItemID
{
    NONE = 0,
    DUMMY_ITEM = 1,
    ID_CARD = 2,
    ID_CARD_V2 = 3,
    ID_CARD_V3 = 4
}

public class ItemContextActionBuilder
{
    public static List<AContextAction> BuilItemContextActions(Item item)
    {
        switch (item.ItemID)
        {
            case ItemID.ID_CARD:
                return new List<AContextAction>() { new GiveAction(item, new DummyContextAction(null)) };
            case ItemID.ID_CARD_V2:
                return new List<AContextAction>() { new GiveAction(item, new DummyContextAction(null)) };
        }
        return new List<AContextAction>();
    }
}
#endregion

#region Context Action Wheel Configuration
public class ContextActionWheelNodeConfigurationData
{
    public static string ICONS_BASE_PATH = "ContextAction/Icons/";
    private Sprite contextActionWheelIcon;

    public ContextActionWheelNodeConfigurationData(Type contextActionType)
    {
        contextActionWheelIcon = Resources.Load<Sprite>(ICONS_BASE_PATH + contextActionType.Name + "_icon");
    }

    public Sprite ContextActionWheelIcon { get => contextActionWheelIcon; }
}

public static class ContextActionWheelNodeConfiguration
{
    public static Dictionary<ContextActionWheelNodeConfigurationId, ContextActionWheelNodeConfigurationData> contextActionWheelNodeConfiguration =
        new Dictionary<ContextActionWheelNodeConfigurationId, ContextActionWheelNodeConfigurationData>()
        {
            {ContextActionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, new ContextActionWheelNodeConfigurationData(typeof(GrabAction)) },
            {ContextActionWheelNodeConfigurationId.GIVE_CONTEXT_ACTION_WHEEL_CONFIG, new ContextActionWheelNodeConfigurationData(typeof(GiveAction)) },
            {ContextActionWheelNodeConfigurationId.TALK_CONTEXT_ACTION_WHEEL_CONFIG, new ContextActionWheelNodeConfigurationData(typeof(TalkAction)) }
        };
}

public enum ContextActionWheelNodeConfigurationId
{
    GRAB_CONTEXT_ACTION_WHEEL_CONFIG = 0,
    GIVE_CONTEXT_ACTION_WHEEL_CONFIG = 1,
    TALK_CONTEXT_ACTION_WHEEL_CONFIG = 2
}
#endregion