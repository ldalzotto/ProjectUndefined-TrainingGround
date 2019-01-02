using System;
using UnityEngine;

public class ContextActionIconResolver
{
    public static string ICONS_BASE_PATH = "ContextAction/Icons/";
    public static Sprite ResolveIcon(Type contextActionType)
    {
        return Resources.Load<Sprite>(ICONS_BASE_PATH + contextActionType.Name + "_icon");
    }
}