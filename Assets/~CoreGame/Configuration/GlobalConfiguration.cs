
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

#endregion