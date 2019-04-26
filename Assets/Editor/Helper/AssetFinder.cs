#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;

public class AssetFinder
{

    public static T SafeSingleAssetFind<T>(string filter) where T : UnityEngine.Object
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

    public static List<T> SafeAssetFind<T>(string filter) where T : UnityEngine.Object
    {
        var foundAssets = AssetDatabase.FindAssets(filter);
        if (foundAssets != null && foundAssets.Length > 0 && foundAssets[0] != null)
        {
            var returnlist = new List<T>();

            foreach(var foundAsset in foundAssets)
            {
                var foundAssetPath = AssetDatabase.GUIDToAssetPath(foundAsset);
                if (foundAssetPath != null)
                {
                    returnlist.Add(AssetDatabase.LoadAssetAtPath<T>(foundAssetPath));
                }
            }
            
        }

        return new List<T>();

    }
}

#endif //UNITY_EDITOR