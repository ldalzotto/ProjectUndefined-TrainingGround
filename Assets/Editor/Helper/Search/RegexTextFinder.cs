using UnityEngine;
using System.Collections;
using UnityEditor.IMGUI.Controls;
using System.Text.RegularExpressions;
using UnityEditor;

public class RegexTextFinder
{
    private SearchField searchField;
    private Regex regex;
    private string searchText;

    public RegexTextFinder()
    {
        this.searchField = new SearchField();
        this.searchField.SetFocus();
    }

    public void GUITick()
    {
        EditorGUI.BeginChangeCheck();
        this.searchText = this.searchField.OnGUI(this.searchText);
        if (EditorGUI.EndChangeCheck())
        {
            this.regex = new Regex(this.searchText, RegexOptions.IgnoreCase);
        }

    }

    public void SetSearchTest(string searchText)
    {
        this.searchText = searchText;
        this.regex = new Regex(this.searchText, RegexOptions.IgnoreCase);
    }

    public bool IsMatchingWith(string comparisonString)
    {
        return string.IsNullOrEmpty(this.searchText) || this.regex.Match(comparisonString).Success;
    }
}
