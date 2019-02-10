
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
    DUMBSTER = 5,
    CROWBAR = 6,
    SEWER_ENTRANCE = 7,
    SEWER_EXIT = 8
}
#endregion

#region Item Configuration
[System.Serializable]
public enum ItemID
{
    NONE = 0,
    DUMMY_ITEM = 1,
    ID_CARD = 2,
    CROWBAR = 5
}

public class ItemContextActionBuilder
{
    public static List<AContextAction> BuilItemContextActions(Item item)
    {
        switch (item.ItemID)
        {
            case ItemID.ID_CARD:
                return new List<AContextAction>() { new GiveAction(item, null) };
            case ItemID.CROWBAR:
                return new List<AContextAction>() { new InteractAction(item, new CutsceneTimelineAction(CutsceneId.PLAYER_OPEN_SEWER, null)) };
        }
        return new List<AContextAction>();
    }
}

public class ItemReceivedDescriptionTextConstants
{
    public static Dictionary<ItemID, string> ItemReceivedDescriptionText = new Dictionary<ItemID, string>()
    {
        { ItemID.ID_CARD, "A dummy id card."},
        {ItemID.CROWBAR, "A bar." }
    };
}
#endregion

#region Context Action Wheel Configuration
public class SelectionWheelNodeConfigurationData
{
    public static string ICONS_BASE_PATH = "ContextAction/Icons/";
    private Sprite contextActionWheelIcon;

    public SelectionWheelNodeConfigurationData(Type contextActionType)
    {
        contextActionWheelIcon = Resources.Load<Sprite>(ICONS_BASE_PATH + contextActionType.Name + "_icon");
    }

    public Sprite ContextActionWheelIcon { get => contextActionWheelIcon; }
}

public static class SelectionWheelNodeConfiguration
{
    public static Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData> selectionWheelNodeConfiguration =
        new Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>()
        {
            {SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(GrabAction)) },
            {SelectionWheelNodeConfigurationId.GIVE_CONTEXT_ACTION_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(GiveAction)) },
            {SelectionWheelNodeConfigurationId.TALK_CONTEXT_ACTION_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(TalkAction)) },
            {SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(RTPuzzle.LaunchProjectileRTPAction)) }
        };
}

public enum SelectionWheelNodeConfigurationId
{
    GRAB_CONTEXT_ACTION_WHEEL_CONFIG = 0,
    GIVE_CONTEXT_ACTION_WHEEL_CONFIG = 1,
    TALK_CONTEXT_ACTION_WHEEL_CONFIG = 2,
    THROW_PLAYER_PUZZLE_WHEEL_CONFIG = 3
}
#endregion