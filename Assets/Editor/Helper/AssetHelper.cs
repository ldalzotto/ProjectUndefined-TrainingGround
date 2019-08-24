using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;

public static class AssetHelper 
{
    public static ScriptableObject CreateAssetAtSameDirectoryLevel(ScriptableObject initialAsset, Type typeToInstanciate, string assetName)
    {
        return CreateAssetAtSameDirectoryLevel(initialAsset, typeToInstanciate.Name, assetName);
    }

    public static ScriptableObject CreateAssetAtSameDirectoryLevel(ScriptableObject initialAsset, string typeToInstanciate, string assetName)
    {
        var baseAssetPath = AssetDatabase.GetAssetPath(initialAsset);
        var createdObject = ScriptableObject.CreateInstance(typeToInstanciate);
        AssetDatabase.CreateAsset(createdObject, Path.GetDirectoryName(baseAssetPath) + "/" + Path.GetFileNameWithoutExtension(baseAssetPath) + "_" + assetName + ".asset");
        return createdObject;
    }
}
