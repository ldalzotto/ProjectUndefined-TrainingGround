#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

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


#endif