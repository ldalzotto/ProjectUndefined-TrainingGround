﻿#if UNITY_EDITOR

using UnityEditor;

public class AssetFinder
{

    public static T SafeSingeAssetFind<T>(string filter) where T : UnityEngine.Object
    {
        var foundAssets = AssetDatabase.FindAssets(filter);
        if (foundAssets != null && foundAssets.Length > 0 && foundAssets[0] != null)
        {
            var firstFoundAssetPath = AssetDatabase.GUIDToAssetPath(foundAssets[0]);
            if (firstFoundAssetPath != null)
            {
                return AssetDatabase.LoadAssetAtPath<T>(firstFoundAssetPath);
            }
        }

        return null;
    }
}

#endif //UNITY_EDITOR