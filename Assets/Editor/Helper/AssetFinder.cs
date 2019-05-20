﻿#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;

public class AssetFinder
{

    public static T SafeSingleAssetFind<T>(string filter) where T : UnityEngine.Object
    {
        var foundAssets = AssetDatabase.FindAssets(filter);
        if (foundAssets != null && foundAssets.Length > 0 && foundAssets[0] != null)
        {
            for (var i = 0; i < foundAssets.Length; i++)
            {
                if (foundAssets[i] != null)
                {
                    var firstFoundAssetPath = AssetDatabase.GUIDToAssetPath(foundAssets[i]);
                    if (firstFoundAssetPath != null)
                    {
                        var loadedAsset = AssetDatabase.LoadAssetAtPath<T>(firstFoundAssetPath);
                        if (loadedAsset != null)
                        {
                            return loadedAsset;
                        }
                    }
                }
            }
        }

        return null;
    }

    public static void SafeSingleAssetFind<T>(ref T property, string filter) where T : UnityEngine.Object
    {
        if (property == null)
        {
            property = SafeSingleAssetFind<T>(filter);
        }
    }

    public static List<T> SafeAssetFind<T>(string filter) where T : UnityEngine.Object
    {
        var foundAssets = AssetDatabase.FindAssets(filter);
        if (foundAssets != null && foundAssets.Length > 0 && foundAssets[0] != null)
        {
            var returnlist = new List<T>();

            foreach (var foundAsset in foundAssets)
            {
                var foundAssetPath = AssetDatabase.GUIDToAssetPath(foundAsset);
                if (foundAssetPath != null)
                {
                    returnlist.Add(AssetDatabase.LoadAssetAtPath<T>(foundAssetPath));
                }
            }

            return returnlist;

        }

        return new List<T>();

    }
}

#endif //UNITY_EDITOR