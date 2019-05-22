#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class CreateableScriptableObjectComponent<T> : CreationModuleComponent, ICreateable where T : ScriptableObject
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

    private Editor scriptableObjectEditor;
    private Object lastFrameObject;
    
    protected virtual string objectFieldLabel { get; }



    public bool IsNew { get => isNew; }
    public T CreatedObject { get => createdObject; set => createdObject = value; }
    public GeneratedScriptableObjectManager<T> GenereatedAsset { get => genereatedAsset; }

    #region External Event
    public override void OnGenerationEnd()
    {
        this.isNew = false;
    }
    #endregion

    protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
    {
        if (GUILayout.Button(new GUIContent("N"), EditorStyles.miniButton, GUILayout.Width(20f)))
        {
            this.InstanciateInEditor(editorProfile);
        }
        EditorGUI.BeginChangeCheck();
        string labelField = this.GetType().Name;
        if(this.objectFieldLabel != null)
        {
            labelField = this.objectFieldLabel;
        }
        this.createdObject = (T)EditorGUILayout.ObjectField(labelField, this.createdObject, typeof(T), false);
        if (EditorGUI.EndChangeCheck())
        {
            this.isNew = false;
        }
        if (this.createdObject != null)
        {
            if (!this.isNew)
            {
                EditorGUI.BeginDisabledGroup(true);
            }
            this.ScriptableObjectGUI(this.createdObject, new SerializedObject(this.createdObject));
            if (!this.isNew)
            {
                EditorGUI.EndDisabledGroup();
            }
        }
    }

    protected virtual void ScriptableObjectGUI(T obj, SerializedObject sObj)
    {
        if (this.lastFrameObject == null || this.lastFrameObject != obj)
        {
            this.scriptableObjectEditor = Editor.CreateEditor(obj);
        }
        this.lastFrameObject = obj;
        if (this.scriptableObjectEditor != null)
        {
            this.scriptableObjectEditor.OnInspectorGUI();
        }
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

    public void InstanciateInEditor(AbstractCreationWizardEditorProfile editorProfile)
    {
        this.createdObject = ScriptableObject.CreateInstance<T>();
        this.isNew = true;
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