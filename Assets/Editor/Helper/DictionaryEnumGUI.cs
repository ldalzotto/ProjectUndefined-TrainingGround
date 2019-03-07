#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;



[System.Serializable]
public class DictionaryEnumGUI<K, V> where K : Enum where V : ScriptableObject
{

    #region Add Entry
    private K idToAdd;
    #endregion

    #region  Sensitive Operations
    [SerializeField]
    private bool sensitiveOperationsEnabled;
    #endregion

    #region Search Field
    private SearchField keySearchField;
    private string keySearchString;
    private GUIStyle keySearchFieldStyle;
    private bool alphabeticalOrder;
    [SerializeField]
    private bool searchFilterFoldout;
    [SerializeField]
    private List<FilterContainsElement> containsValues;
    [SerializeField]
    private FilterFoldableReordableList<FilterContainsElement> containsValuesReorderable;
    #endregion

    #region Modifying Entry
    private Dictionary<K, V> valuesToSet = new Dictionary<K, V>();
    private Dictionary<K, V> valuesToRemove = new Dictionary<K, V>();
    #endregion

    #region Entry Look
    [SerializeField]
    private List<K> valuesToLook;
    private bool forceLookOfAll;
    private GUIStyle removeButtonDeactivatedStyle;
    #endregion

    public void GUITick(ref Dictionary<K, V> dictionaryEditorValues)
    {
        DoInit(ref dictionaryEditorValues);
        DoAddEntry(ref dictionaryEditorValues);

        EditorGUILayout.LabelField(typeof(V).Name + " : ", EditorStyles.boldLabel);

        DoSearchFilter(ref dictionaryEditorValues);

        var dictionaryEditorEntryValues = dictionaryEditorValues.ToList();
        if (this.alphabeticalOrder)
        {
            dictionaryEditorEntryValues = dictionaryEditorValues.OrderBy(kv => kv.Key.ToString()).ToList();
        }

        int displayedCounter = 0;
        foreach (var dictionaryEditorEntry in dictionaryEditorEntryValues)
        {
            //search filter
            if ((this.keySearchString == null || this.keySearchString == "" || dictionaryEditorEntry.Key.ToString().ToLower().Contains(this.keySearchString.ToLower()))
                && (this.containsValues == null || this.containsValues.Count == 0 || !this.containsValuesReorderable.IsFilterEnabled || FilterContainsElement.ToElements(this.containsValues).Contains(dictionaryEditorEntry.Value)))
            {
                DoDisplayEntry(ref dictionaryEditorValues, dictionaryEditorEntry);
                displayedCounter += 1;
                if (this.valuesToLook.Contains(dictionaryEditorEntry.Key))
                {
                    if (dictionaryEditorEntry.Value != null)
                    {
                        Editor.CreateEditor(dictionaryEditorEntry.Value).OnInspectorGUI();
                    }
                }
            }
        }

        if (displayedCounter == 0)
        {
            EditorGUILayout.LabelField("No elements found.");
        }

        if (valuesToSet.Count > 0)
        {
            foreach (var valueToSet in valuesToSet)
            {
                dictionaryEditorValues[valueToSet.Key] = valueToSet.Value;
            }
            valuesToSet.Clear();
        }
        if (valuesToRemove.Count > 0)
        {
            foreach (var valueToRemove in valuesToRemove)
            {
                dictionaryEditorValues.Remove(valueToRemove.Key);
            }
            valuesToRemove.Clear();
        }

    }

    private void DoInit(ref Dictionary<K, V> dictionaryEditorValues)
    {
        if (this.valuesToLook == null)
        {
            this.valuesToLook = new List<K>();
        }

        if (dictionaryEditorValues == null)
        {
            dictionaryEditorValues = new Dictionary<K, V>();
        }
        if (keySearchField == null)
        {
            keySearchField = new SearchField();
            this.keySearchFieldStyle = new GUIStyle();
            this.keySearchFieldStyle.padding = EditorStyles.miniButton.padding;
        }
        if (containsValues == null)
        {
            containsValues = new List<FilterContainsElement>();
        }
        if (containsValuesReorderable == null)
        {
            containsValuesReorderable = new FilterFoldableReordableList<FilterContainsElement>(this.containsValues, true, true, true, true, "Contains : ", 1,
                (Rect rect, int index, bool isActive, bool isFocused) => { FilterContainsElement.DrawElement(rect, this.containsValues[index]); });
        }
        if (removeButtonDeactivatedStyle == null)
        {
            removeButtonDeactivatedStyle = new GUIStyle(EditorStyles.miniButtonRight);
            removeButtonDeactivatedStyle.normal = EditorStyles.miniButtonRight.active;
        }
    }

    private void DoAddEntry(ref Dictionary<K, V> dictionaryEditorValues)
    {
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Add " + typeof(V).Name + " configuration : ", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+", EditorStyles.miniButton))
        {
            if (!dictionaryEditorValues.ContainsKey(this.idToAdd))
            {
                dictionaryEditorValues.Add(this.idToAdd, null);
            }
        }
        this.idToAdd = (K)EditorGUILayout.EnumPopup(idToAdd);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
    }

    private void DoSearchFilter(ref Dictionary<K, V> dictionaryEditorValues)
    {
        EditorGUILayout.BeginVertical(this.keySearchFieldStyle);

        this.keySearchString = this.keySearchField.OnGUI(keySearchString);

        EditorGUILayout.Separator();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(30f));
        this.sensitiveOperationsEnabled = GUILayout.Toggle(this.sensitiveOperationsEnabled, new GUIContent("!", "Authorize sensitive operations."), EditorStyles.miniButtonLeft);
        this.alphabeticalOrder = GUILayout.Toggle(this.alphabeticalOrder, new GUIContent("A↑", "Alphabetical order."), EditorStyles.miniButtonMid);
        EditorGUI.BeginChangeCheck();
        this.forceLookOfAll = GUILayout.Toggle(this.forceLookOfAll, new GUIContent("L*", "Show all elements detail."), EditorStyles.miniButtonRight);
        if (EditorGUI.EndChangeCheck()) {
            if (this.forceLookOfAll)
            {
                this.valuesToLook = dictionaryEditorValues.Keys.ToList();
            } else
            {
                this.valuesToLook.Clear();
            }
         
        } 
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical(EditorStyles.textField);
        EditorGUILayout.BeginHorizontal();
        this.searchFilterFoldout = EditorGUILayout.Foldout(this.searchFilterFoldout, "Filters", EditorStyles.foldout);
        EditorGUILayout.EndHorizontal();
        if (this.searchFilterFoldout)
        {
            containsValuesReorderable.DoLayoutList();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
    }

    private void DoDisplayEntry(ref Dictionary<K, V> dictionaryEditorValues, KeyValuePair<K, V> dictionaryEditorEntry)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        var selectedNewKey = (K)EditorGUILayout.EnumPopup(dictionaryEditorEntry.Key);
        if (EditorGUI.EndChangeCheck())
        {
            if (!dictionaryEditorValues.ContainsKey(selectedNewKey))
            {
                valuesToSet[selectedNewKey] = dictionaryEditorEntry.Value;
                valuesToRemove[dictionaryEditorEntry.Key] = dictionaryEditorEntry.Value;
            }
        }

        EditorGUI.BeginChangeCheck();
        var configurationObjectField = EditorGUILayout.ObjectField(dictionaryEditorEntry.Value, typeof(V), false) as V;
        if (EditorGUI.EndChangeCheck())
        {
            valuesToSet[dictionaryEditorEntry.Key] = configurationObjectField;
        }

        var lookPressed = (this.valuesToLook.Contains(dictionaryEditorEntry.Key));
        lookPressed = GUILayout.Toggle(lookPressed, new GUIContent("L", "View element details."), EditorStyles.miniButtonLeft);

        if (!this.forceLookOfAll)
        {
            if (lookPressed)
            {
                if (!this.valuesToLook.Contains(dictionaryEditorEntry.Key))
                {
                    this.valuesToLook.Add(dictionaryEditorEntry.Key);
                }
            }
            else
            {
                if (this.valuesToLook.Contains(dictionaryEditorEntry.Key))
                {
                    this.valuesToLook.Remove(dictionaryEditorEntry.Key);
                }
            }
        }

        GUIStyle buttonStyle = EditorStyles.miniButtonRight;
        if (!sensitiveOperationsEnabled)
        {
            buttonStyle = this.removeButtonDeactivatedStyle;
        }
        if (GUILayout.Button(new GUIContent("-", "Delete element."), buttonStyle))
        {
            if (sensitiveOperationsEnabled)
            {
                valuesToRemove[dictionaryEditorEntry.Key] = configurationObjectField;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    #region custom filter data 
    [System.Serializable]
    class FilterContainsElement
    {
        [SerializeField]
        private V containsElement;

        public V ContainsElement { get => containsElement; set => containsElement = value; }

        public static void DrawElement(Rect rect, FilterContainsElement element)
        {
            element.ContainsElement = EditorGUI.ObjectField(rect, element.ContainsElement, typeof(V), false) as V; ;
        }

        public static List<V> ToElements(List<FilterContainsElement> filterContainsElements)
        {
            return filterContainsElements.ConvertAll<V>((e) => e.containsElement);
        }

    }
    #endregion
}

#endif //UNITY_EDITOR