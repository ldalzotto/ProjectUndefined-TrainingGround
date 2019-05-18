using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CreateablePrefabComponent<S> : CreationModuleComponent where S : UnityEngine.Object
{
    private GUIStyle selectionStyle;

    [SerializeField]
    private bool selectionToggle;

    private GUIStyle newStyle;

    [SerializeField]
    private bool newToggle;

    [SerializeField]
    private S createdPrefab;

    private GeneratedPrefabAssetManager<S> generatedPrefabAssetManager;

    private S BasePrefab;

    public abstract Func<Dictionary<string, CreationModuleComponent>, S> BasePrefabProvider { get; }
    public S CreatedPrefab { get => createdPrefab; }

    protected CreateablePrefabComponent(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
    {
    }

    protected override void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules)
    {
        if (selectionStyle == null)
        {
            selectionStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        }
        if (newStyle == null)
        {
            newStyle = new GUIStyle(EditorStyles.miniButtonRight);
        }

        EditorGUILayout.BeginHorizontal();
        this.selectionToggle = GUILayout.Toggle(this.selectionToggle, new GUIContent("S"), selectionStyle, GUILayout.Width(20f));
        if (this.selectionToggle)
        {
            this.newToggle = false;
        }
        this.newToggle = GUILayout.Toggle(this.newToggle, new GUIContent("N"), newStyle, GUILayout.Width(20f));
        if (this.newToggle)
        {
            this.selectionToggle = false;
        }
        EditorGUILayout.EndHorizontal();

        if (this.selectionToggle)
        {
            this.createdPrefab = EditorGUILayout.ObjectField("Select " + typeof(S).Name, this.createdPrefab, typeof(S), false) as S;
        }
        else if (this.newToggle)
        {
            if (this.createdPrefab == null && this.BasePrefabProvider != null)
            {
                if (this.BasePrefab == null)
                {
                    this.BasePrefab = this.BasePrefabProvider.Invoke(editorModules);
                }
                this.createdPrefab = PrefabUtility.LoadPrefabContents(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.BasePrefab)).GetComponent<S>();
            }
            Editor.CreateEditor(this.createdPrefab).OnInspectorGUI();
        }
    }

    public bool IsNew()
    {
        return this.newToggle;
    }
    public bool IsSelected()
    {
        return this.selectionToggle;
    }

    public S Create(string basePath, string baseName)
    {
        if (this.IsNew())
        {
            this.generatedPrefabAssetManager = new GeneratedPrefabAssetManager<S>(this.BasePrefab, basePath, baseName);
            DestroyImmediate(this.createdPrefab);
            this.createdPrefab = this.generatedPrefabAssetManager.SavedAsset.GetComponent<S>();
        }
        return this.createdPrefab;
    }

    public void MoveGeneratedAsset(string targetPath)
    {
        if (this.IsNew())
        {
            this.generatedPrefabAssetManager.MoveGeneratedAsset(targetPath);
        }
    }

    public override void ResetEditor()
    {
        this.createdPrefab = null;
    }

    public void OnGenerationEnd()
    {
        this.newToggle = false;
        this.selectionToggle = true;
    }


}

public interface CreatablePrefabInput
{
}