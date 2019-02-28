
using System;
using System.Collections.Generic;
using UnityEngine;

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
            {SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(RTPuzzle.LaunchProjectileAction)) }
        };
}

public enum SelectionWheelNodeConfigurationId
{
    THROW_PLAYER_PUZZLE_WHEEL_CONFIG = 3
}
#endregion