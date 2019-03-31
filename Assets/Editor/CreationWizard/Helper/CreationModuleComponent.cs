using OdinSerializer;
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

    protected abstract string foldoutLabel { get; }

    protected CreationModuleComponent(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble)
    {
        ModuleFoldout = moduleFoldout;
        ModuleEnabled = moduleEnabled;
        this.ModuleDisableAble = moduleDisableAble;
    }

    public void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        EditorGUILayout.BeginHorizontal();
        var newModuleEnabled = GUILayout.Toggle(this.ModuleEnabled, "", EditorStyles.miniButton, GUILayout.Width(10), GUILayout.Height(10));

        if (ModuleDisableAble)
        {
            this.ModuleEnabled = newModuleEnabled;
        }

        this.ModuleFoldout = EditorGUILayout.Foldout(this.ModuleFoldout, this.foldoutLabel, true);
        EditorGUILayout.EndHorizontal();

        if (this.ModuleFoldout)
        {
            this.OnInspectorGUIImpl();
        }
        EditorGUILayout.EndVertical();
    }

    protected abstract void OnInspectorGUIImpl();
    public abstract void ResetEditor();
}
