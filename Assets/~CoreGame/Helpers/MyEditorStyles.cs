
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class MyEditorStyles
{
    public static GUIStyle LabelRed;
    public static GUIStyle LabelYellow;
    public static GUIStyle LabelMagenta;
    public static GUIStyle LabelBlue;
    public static GUIStyle LabelGreen;

    static MyEditorStyles()
    {
        MyEditorStyles.LabelRed = BuildLabelStyle(Color.red);
        MyEditorStyles.LabelYellow = BuildLabelStyle(Color.yellow);
        MyEditorStyles.LabelMagenta = BuildLabelStyle(Color.magenta);
        MyEditorStyles.LabelBlue = BuildLabelStyle(Color.blue);
        MyEditorStyles.LabelGreen = BuildLabelStyle(Color.green);
    }

    public static GUIStyle BuildLabelStyle(Color color)
    {
        var label = new GUIStyle(EditorStyles.label);
        label.normal.textColor = color;
        return label;
    }
}

#endif

public class MyColors
{
    public static Color TransparentBlack;
    public static Color PaleBlue;
    public static Color Coral;
    public static Color HotPink;

    static MyColors()
    {
        MyColors.TransparentBlack = new Color(0, 0, 0, 0);
        MyColors.PaleBlue = new Color(0.709f, 0.827f, 0.905f);
        MyColors.Coral = new Color(1f, 127f / 255f, 80f / 255f);
        MyColors.HotPink = new Color(1f, 105f / 255f, 180f / 255f);
    }
}
