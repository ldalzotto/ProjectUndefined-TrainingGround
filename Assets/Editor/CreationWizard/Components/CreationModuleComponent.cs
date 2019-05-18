using OdinSerializer;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class CreationModuleComponent : SerializedScriptableObject
{
    [HideInInspector]
    public bool ModuleFoldout;

    [HideInInspector]
    public bool ModuleEnabled;

    [HideInInspector]
    public bool ModuleDisableAble;

    [HideInInspector]
    public string TmpFolderPath;

    private string warningMessage;
    private string errorMessage;
    private GUIStyle foldoutStyle;


    protected virtual string foldoutLabel { get; }
    protected virtual string headerDescriptionLabel { get; }

    public static T Create<T>(string filePath, bool moduleFoldout, bool moduleEnabled, bool moduleDistableAble, string tmpFolderPath) where T : CreationModuleComponent
    {
        var instance = Create<T>(filePath);
        instance.ModuleFoldout = moduleFoldout;
        instance.ModuleEnabled = moduleEnabled;
        instance.ModuleDisableAble = moduleDistableAble;
        instance.TmpFolderPath = tmpFolderPath;
        return instance;
    }

    public static T Create<T>(string filePath) where T : CreationModuleComponent
    {
        var instance = ScriptableObject.CreateInstance<T>();
        instance.ModuleFoldout = false;
        instance.ModuleEnabled = true;
        instance.ModuleDisableAble = false;
        AssetDatabase.CreateAsset(instance, filePath);
        return instance;
    }

    protected CreationModuleComponent(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble)
    {
        ModuleFoldout = moduleFoldout;
        ModuleEnabled = moduleEnabled;
        this.ModuleDisableAble = moduleDisableAble;
    }

    public void OnInspectorGUI(ref Dictionary<string, CreationModuleComponent> editorModules)
    {
        this.DoInit();

        if (!string.IsNullOrEmpty(this.errorMessage))
        {
            this.SetFoldoutStyleTextColor(ref this.foldoutStyle, Color.red);
        }
        else if (!string.IsNullOrEmpty(this.warningMessage))
        {
            this.SetFoldoutStyleTextColor(ref this.foldoutStyle, Color.yellow);
        }
        else
        {
            this.SetFoldoutStyleTextColor(ref this.foldoutStyle, Color.black);
        }

        this.warningMessage = this.ComputeWarningState(ref editorModules);
        this.errorMessage = this.ComputeErrorState(ref editorModules);
        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        EditorGUILayout.BeginHorizontal();
        var newModuleEnabled = GUILayout.Toggle(this.ModuleEnabled, "", EditorStyles.miniButton, GUILayout.Width(10), GUILayout.Height(10));

        if (ModuleDisableAble)
        {
            this.ModuleEnabled = newModuleEnabled;
        }

        var displayedFoldoutLabel = this.foldoutLabel;
        if (displayedFoldoutLabel == null)
        {
            displayedFoldoutLabel = this.GetType().Name + " : ";
        }
        this.ModuleFoldout = EditorGUILayout.Foldout(this.ModuleFoldout, displayedFoldoutLabel, true, this.foldoutStyle);

        EditorGUILayout.EndHorizontal();

        if (this.ModuleFoldout)
        {
            var serializedObject = new SerializedObject(this);
            EditorGUILayout.LabelField(this.headerDescriptionLabel, EditorStyles.miniLabel);
            //GUILayout.Label(new GUIContent(this.headerDescriptionLabel), EditorStyles.miniLabel);

            this.OnInspectorGUIImpl(serializedObject, ref editorModules);
            serializedObject.ApplyModifiedProperties();

            if (!string.IsNullOrEmpty(this.errorMessage))
            {
                EditorGUILayout.HelpBox(this.errorMessage, MessageType.Error, true);
            }
            else if (!string.IsNullOrEmpty(this.warningMessage))
            {
                EditorGUILayout.HelpBox(this.warningMessage, MessageType.Warning, true);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DoInit()
    {
        if (this.foldoutStyle == null)
        {
            this.foldoutStyle = new GUIStyle(EditorStyles.foldout);
        }
    }

    protected abstract void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules);
    public abstract void ResetEditor();
    public virtual string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules) { return string.Empty; }
    public virtual string ComputeErrorState(ref Dictionary<string, CreationModuleComponent> editorModules) { return string.Empty; }

    private void SetFoldoutStyleTextColor(ref GUIStyle style, Color textColor)
    {
        style.normal.textColor = textColor;
        style.onNormal.textColor = textColor;
        style.hover.textColor = textColor;
        style.onHover.textColor = textColor;
        style.focused.textColor = textColor;
        style.onFocused.textColor = textColor;
        style.active.textColor = textColor;
        style.onActive.textColor = textColor;
    }

    public bool HasWarning()
    {
        return !string.IsNullOrEmpty(this.warningMessage);
    }

    public bool HasError()
    {
        return !string.IsNullOrEmpty(this.errorMessage);
    }

}
