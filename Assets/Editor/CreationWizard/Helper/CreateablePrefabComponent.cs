using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CreateablePrefabComponent<N, S> : CreationModuleComponent where N : UnityEngine.Object where S : UnityEngine.Object
{
    private GUIStyle selectionStyle;

    [SerializeField]
    private bool selectionToggle;
    
    private GUIStyle newStyle;

    [SerializeField]
    private bool newToggle;

    [SerializeField]
    private N newPrefab;

    [SerializeField]
    private S selectionPrefab;

    private GeneratedPrefabAssetManager<S> generatedPrefabAssetManager;

    public N NewPrefab { get => newPrefab;  }
    public S SelectionPrefab { get => selectionPrefab; }

    protected CreateablePrefabComponent(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
    {
    }

    protected override void OnInspectorGUIImpl()
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
            this.selectionPrefab = EditorGUILayout.ObjectField("Select " + typeof(S).Name, this.selectionPrefab, typeof(S), false) as S;
        }
        else if (this.newToggle)
        {
            this.newPrefab = EditorGUILayout.ObjectField("Select " + typeof(N).Name, this.newPrefab, typeof(N), false) as N;
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

    public S Create(S basePrefab, Scene tmpScene, string basePath, string baseName, Action<S, N> afterBaseCreation)
    {
        if (this.IsNew())
        {
            this.generatedPrefabAssetManager = new GeneratedPrefabAssetManager<S>(basePrefab, tmpScene, basePath, baseName, (S obj) => {
                afterBaseCreation.Invoke(obj, this.newPrefab);
            });
            return this.generatedPrefabAssetManager.SavedAsset.GetComponent<S>();
        } else
        {
            return this.SelectionPrefab;
        }
    }

    public void MoveGeneratedAsset(string targetPath)
    {
        if (this.IsNew())
        {
            this.generatedPrefabAssetManager.MoveGeneratedAsset(targetPath);
        }
    }

}
