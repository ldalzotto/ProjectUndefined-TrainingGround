using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

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
        SEWER_EXIT = 8,
        SEWER_TO_PUZZLE = 9
    }
    #endregion

    #region Item Configuration

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

    #endregion

    #region Context Action Wheel Configuration
    public static class SelectionWheelNodeConfiguration
    {
        public static Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData> selectionWheelNodeConfiguration =
            new Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>()
            {
            {SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(GrabAction)) },
            {SelectionWheelNodeConfigurationId.GIVE_CONTEXT_ACTION_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(GiveAction)) },
            {SelectionWheelNodeConfigurationId.TALK_CONTEXT_ACTION_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(TalkAction)) }
            };
    }

    public enum SelectionWheelNodeConfigurationId
    {
        GRAB_CONTEXT_ACTION_WHEEL_CONFIG = 0,
        GIVE_CONTEXT_ACTION_WHEEL_CONFIG = 1,
        TALK_CONTEXT_ACTION_WHEEL_CONFIG = 2
    }
    #endregion
}