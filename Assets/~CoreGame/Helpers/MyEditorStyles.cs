﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class MyEditorStyles
{
    public static GUIStyle LabelRed;
    public static GUIStyle LabelYellow;
    public static GUIStyle LabelMagenta;
    public static GUIStyle LabelBlue;
    static MyEditorStyles()
    {
        MyEditorStyles.LabelRed = BuildLabelStyle(Color.red);
        MyEditorStyles.LabelYellow = BuildLabelStyle(Color.yellow);
        MyEditorStyles.LabelMagenta = BuildLabelStyle(Color.magenta);
        MyEditorStyles.LabelBlue = BuildLabelStyle(Color.blue);
    }

    public static GUIStyle BuildLabelStyle(Color color)
    {
        var label = new GUIStyle(EditorStyles.label);
        label.normal.textColor = color;
        return label;
    }
}

public class MyColors
{
    public static Color PaleBlue;
    public static Color Coral;

    static MyColors()
    {
        MyColors.PaleBlue = new Color(0.709f, 0.827f, 0.905f);
        MyColors.Coral = new Color(1f, 127f / 255f, 80f / 255f);
    }
}

#endif