using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

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