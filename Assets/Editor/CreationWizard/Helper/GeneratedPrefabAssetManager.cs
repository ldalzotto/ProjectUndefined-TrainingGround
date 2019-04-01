#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GeneratedPrefabAssetManager<T> where T : UnityEngine.Object
{
    private string generatedFileName;
    private GameObject savedAsset;

    public GeneratedPrefabAssetManager(T originalObject, Scene destinationScene,
        string folderPath, string fileBaseName, Action<T> afterAssetCreation)
    {
        T instance = default(T);
        if (typeof(T).IsAssignableFrom(typeof(GameObject)) || typeof(T).IsSubclassOf(typeof(Component)))
        //   if (prefabAssetGenerationWorkflow == PrefabAssetGenerationWorkflow.PREFAB)
        {
            instance = (T)PrefabUtility.InstantiatePrefab(originalObject, destinationScene);
        }
        else // if (prefabAssetGenerationWorkflow == PrefabAssetGenerationWorkflow.MODEL)
        {
            var modelObject = (GameObject)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(originalObject));
            instance = (T)PrefabUtility.InstantiatePrefab(modelObject, destinationScene);
        }

        if (afterAssetCreation != null)
        {
            afterAssetCreation.Invoke(instance);
        }

        this.generatedFileName = "\\" + fileBaseName + ".prefab";
        if (typeof(T).IsAssignableFrom(typeof(GameObject)))
        {
            this.savedAsset = PrefabUtility.SaveAsPrefabAsset((instance as GameObject), folderPath + this.generatedFileName);
        }
        else if (typeof(T).IsSubclassOf(typeof(Component)))
        {
            this.savedAsset = PrefabUtility.SaveAsPrefabAsset((instance as Component).gameObject, folderPath + this.generatedFileName);
        }
        GameObject.DestroyImmediate(instance);
    }

    public GameObject SavedAsset { get => savedAsset; }

    public void MoveGeneratedAsset(string targetPath)
    {
        var oldPath = AssetDatabase.GetAssetPath(this.savedAsset);
        var newPath = targetPath + this.generatedFileName;
        var errorMessage = AssetDatabase.ValidateMoveAsset(oldPath, newPath);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            throw new AssetMoveError(errorMessage);
        }
        AssetDatabase.MoveAsset(oldPath, newPath);
    }
}

public enum PrefabAssetGenerationWorkflow
{
    MODEL, PREFAB
}

public class AssetMoveError : Exception
{
    public AssetMoveError(string message) : base(message)
    {

    }
}
#endif