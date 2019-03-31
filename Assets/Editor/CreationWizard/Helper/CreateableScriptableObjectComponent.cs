#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class CreateableScriptableObjectComponent<T> : CreationModuleComponent where T : ScriptableObject
{
    [SerializeField]
    private CreationModuleComponent module;
    [SerializeField]
    private bool isNew;
    [SerializeField]
    private bool headerFoldout;
    [SerializeField]
    private T createdObject;

    private GeneratedScriptableObjectManager<T> genereatedAsset;

    protected CreateableScriptableObjectComponent(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
    {
    }

    protected abstract string objectFieldLabel { get; }

  

    public bool IsNew { get => isNew; }
    public T CreatedObject { get => createdObject; set => createdObject = value; }
    public GeneratedScriptableObjectManager<T> GenereatedAsset { get => genereatedAsset; }

   // protected override string foldoutLabel => "Object game configuration : ";

    #region External Event
    public void OnGenerationEnd()
    {
        this.isNew = false;
    }
    #endregion

    protected override void OnInspectorGUIImpl()
    {
        if (GUILayout.Button(new GUIContent("N"), EditorStyles.miniButton, GUILayout.Width(20f)))
        {
            this.createdObject = ScriptableObject.CreateInstance<T>();
            this.isNew = true;
        }
        EditorGUI.BeginChangeCheck();
        this.createdObject = (T)EditorGUILayout.ObjectField(this.objectFieldLabel, this.createdObject, typeof(T), false);
        if (EditorGUI.EndChangeCheck())
        {
            this.isNew = false;
        }
        if (this.createdObject != null)
        {
            this.ScriptableObjectGUI(this.createdObject, new SerializedObject(this.createdObject));
        }
    }

    protected virtual void ScriptableObjectGUI(T obj, SerializedObject sObj)
    {
        Editor.CreateEditor(obj).OnInspectorGUI();
    }
    
    public T CreateAsset(string folderPath, string fileBaseName)
    {
        T returnObject = default(T);
        if (this.isNew)
        {
            this.genereatedAsset = new GeneratedScriptableObjectManager<T>(this.createdObject, folderPath, fileBaseName);
            returnObject = this.genereatedAsset.GeneratedAsset;
        }
        else
        {
            returnObject = this.createdObject;
        }
        return returnObject;
    }

    public void MoveGeneratedAsset(string targetPath)
    {
        if (this.isNew)
        {
            this.genereatedAsset.MoveGeneratedAsset(targetPath);
        }
    }

    public override void ResetEditor()
    {
        this.createdObject = null;
    }

}
#endif