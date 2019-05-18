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

public class TreePickerPopup : PopupWindowContent
{

    public const string NAME_SEPARATION = "//";

    #region Search Bars
    private SearchField treeSearchField;
    private string searchString = string.Empty;
    private Regex searchRegex;
    #endregion //Search Bars

    #region Keys configuration
    private List<string> sortedKeys;
    private TreePathNode pathRootNode;
    #endregion

    private Action OnSelectionChange;
    private Action repaintAction;
    private string selectedKey;
    private GUIStyle selectedStyle;
    private Color nonSelectableColor = new Color(0.8f, 0.8f, 0.8f);
    private Vector2 windowDimensions;

    private Vector2 scrollPosition;
    private int currentIndentLevel;

    public override Vector2 GetWindowSize()
    {
        return this.windowDimensions;
    }

    public TreePickerPopup(List<string> sortedKeys, Action OnSelectionChange, string oldSelectedKey)
    {
        this.selectedKey = oldSelectedKey;
        this.sortedKeys = sortedKeys;
        this.OnSelectionChange = OnSelectionChange;
    }

    public string SelectedKey { get => selectedKey; }
    public Action RepaintAction { set => repaintAction = value; }
    public Vector2 WindowDimensions { set => windowDimensions = value; }

    public override void OnGUI(Rect rect)
    {
        this.Init();

        this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical();

        EditorGUI.BeginChangeCheck();
        this.searchString = this.treeSearchField.OnGUI(this.searchString);
        if (EditorGUI.EndChangeCheck())
        {
            this.searchRegex = new Regex(this.searchString);
        }
        EditorGUILayout.Space();

        this.GUITreeDictionary(this.pathRootNode.ChildNodes);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        if (EditorGUI.EndChangeCheck())
        {
            this.repaintAction.Invoke();
        }
    }

    private void GUITreeDictionary(Dictionary<string, TreePathNode> nodes)
    {
        foreach (var k in nodes)
        {
            //filtering
            if (this.searchRegex.Match(k.Value.Key).Success)
            {

                if (k.Value.ChildNodes.Count > 0)
                {
                    var oldBackgroundColor = GUI.backgroundColor;
                    GUI.backgroundColor = this.nonSelectableColor;
                    GUILayout.Button(k.Key.ToString().Insert(0, this.SpaceFromIndentLevel()), this.selectedStyle);
                    GUI.backgroundColor = oldBackgroundColor;
                    this.currentIndentLevel += 1;
                    this.GUITreeDictionary(k.Value.ChildNodes);
                    this.currentIndentLevel -= 1;
                }
                else
                {
                    var oldBackgroundColor = GUI.backgroundColor;
                    if (k.Value.Key == this.selectedKey)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(k.Key.ToString().Insert(0, this.SpaceFromIndentLevel()), this.selectedStyle))
                    {
                        this.selectedKey = k.Value.Key;
                        this.OnSelectionChange.Invoke();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = oldBackgroundColor;
                }
            }
        }
    }

    private void Init()
    {
        if (this.selectedStyle == null)
        {
            this.selectedStyle = new GUIStyle(EditorStyles.label);
            this.selectedStyle.normal.background = Texture2D.whiteTexture;
        }
        if (this.treeSearchField == null)
        {
            this.treeSearchField = new SearchField();
        }
        if (this.searchRegex == null)
        {
            this.searchRegex = new Regex(this.searchString);
        }
        if (this.pathRootNode == null)
        {
            this.BuildPathNodes();
        }
        this.currentIndentLevel = 0;
    }

    private void BuildPathNodes()
    {
        this.pathRootNode = new TreePathNode(new Dictionary<string, TreePathNode>(), string.Empty);
        foreach (var key in this.sortedKeys)
        {
            var paths = this.ExtractKeyPath(key);
            paths.Reverse();
            var pathStack = new Stack<string>(paths);
            this.pathRootNode.InsertPath(ref pathStack, key);
        }
    }

    private List<string> ExtractKeyPath(string key)
    {
        return key.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private string SpaceFromIndentLevel()
    {
        var str = string.Empty;
        for (var i = 0; i < this.currentIndentLevel; i++)
        {
            str += "  ";
        }
        return str;
    }

}

public class TreePathNode
{
    public Dictionary<string, TreePathNode> ChildNodes;
    public string Key;

    public TreePathNode(Dictionary<string, TreePathNode> childNodes, string key)
    {
        ChildNodes = childNodes;
        Key = key;
    }

    public void InsertPath(ref Stack<string> pathStack, string key)
    {
        if (pathStack != null && pathStack.Count > 0)
        {
            var currentPath = pathStack.Pop();
            if (!this.ChildNodes.ContainsKey(currentPath))
            {
                this.ChildNodes.Add(currentPath, new TreePathNode(new Dictionary<string, TreePathNode>(), key));
                this.ChildNodes[currentPath].InsertPath(ref pathStack, key);
            }
            else
            {
                this.ChildNodes[currentPath].InsertPath(ref pathStack, key);
            }
        }
    }

}