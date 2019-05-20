using UnityEngine;
using System.Collections;
using UnityEditor.IMGUI.Controls;
using OdinSerializer;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using System.Linq;
using System;
using System.Text.RegularExpressions;

[System.Serializable]
public abstract class TreeChoiceHeaderTab<T> : SerializedScriptableObject
{
    public abstract Dictionary<string, T> Configurations { get; }

    public Vector2 TreePickerPopupWindowDimensions = new Vector2(300, 400);
    private TreePickerPopup TreePickerPopup;
    [SerializeField]
    private string selectedKey;

    private GUIStyle selectedItemLabelStyle;
    private Rect buttonRect;
    public void GUITick(Action repaintAction)
    {
        Init(repaintAction);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("S", EditorStyles.miniButton, GUILayout.Width(20)))
        {
            PopupWindow.Show(this.buttonRect, this.TreePickerPopup);
        }
        if (!string.IsNullOrEmpty(this.selectedKey))
        {
            var oldBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            EditorGUILayout.LabelField(this.selectedKey, this.selectedItemLabelStyle);
            GUI.backgroundColor = oldBackgroundColor;
        }
        EditorGUILayout.EndHorizontal();
        if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
    }

    private void Init(Action repaintAction)
    {
        if (this.selectedItemLabelStyle == null)
        {
            this.selectedItemLabelStyle = new GUIStyle(EditorStyles.label);
            this.selectedItemLabelStyle.normal.background = Texture2D.whiteTexture;
        }
        if (this.TreePickerPopup == null)
        {
            var sortedKeys = this.Configurations.Keys.ToList();
            sortedKeys.Sort();
            this.TreePickerPopup = new TreePickerPopup(sortedKeys, OnSelectionChange: () => { this.selectedKey = this.TreePickerPopup.SelectedKey; }, this.selectedKey);
        }
        this.TreePickerPopup.RepaintAction = repaintAction;
        this.TreePickerPopup.WindowDimensions = this.TreePickerPopupWindowDimensions;

    }

    public T GetSelectedConf()
    {
        if (!string.IsNullOrEmpty(this.selectedKey))
        {
            return this.Configurations[this.selectedKey];
        }
        return default;
    }

    public void SetSelectedKey(string newSelectedKey)
    {
        this.selectedKey = newSelectedKey;
    }
}
