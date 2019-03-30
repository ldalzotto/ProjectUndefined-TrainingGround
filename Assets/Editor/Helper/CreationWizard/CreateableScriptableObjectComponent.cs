#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class CreateableScriptableObjectComponent<T> where T : ScriptableObject
{
    [SerializeField]
    private bool isNew;
    [SerializeField]
    private bool headerFoldout;
    [SerializeField]
    private T createdObject;

    private GeneratedScriptableObjectManager<T> genereatedAsset;

    public bool IsNew { get => isNew; }
    public T CreatedObject { get => createdObject; set => createdObject = value; }
    public GeneratedScriptableObjectManager<T> GenereatedAsset { get => genereatedAsset; }

    #region External Event
    public void OnGenerationEnd()
    {
        this.isNew = false;
    }
    #endregion

    public void OnGui()
    {
        #region Attractive object game configuration
        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        this.headerFoldout = EditorGUILayout.Foldout(this.headerFoldout, "Object game configuration : ", true);
        if (this.headerFoldout)
        {
            if (GUILayout.Button(new GUIContent("N"), EditorStyles.miniButton, GUILayout.Width(20f)))
            {
                this.createdObject = ScriptableObject.CreateInstance<T>();
                this.isNew = true;
            }
            EditorGUI.BeginChangeCheck();
            this.createdObject = (T)EditorGUILayout.ObjectField("Generated attractive object configuration : ", this.createdObject, typeof(T), false);
            if (EditorGUI.EndChangeCheck())
            {
                this.isNew = false;
            }
            if (this.createdObject != null)
            {
                Editor.CreateEditor(this.createdObject).OnInspectorGUI();
            }
        }

        EditorGUILayout.EndVertical();
        #endregion
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
        if(this.isNew)
        {
            this.genereatedAsset.MoveGeneratedAsset(targetPath);
        }
    }
}
#endif