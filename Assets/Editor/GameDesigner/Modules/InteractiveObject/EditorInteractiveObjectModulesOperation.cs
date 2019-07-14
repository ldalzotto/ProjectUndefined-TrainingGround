using RTPuzzle;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    public static class EditorInteractiveObjectModulesOperation
    {

        public static T AddPrefabModule<T>(InteractiveObjectType interactiveObjectType, T modulePrefab) where T : InteractiveObjectModule
        {
            T foundPrefabModule = interactiveObjectType.GetComponentInChildren<T>();
            if (foundPrefabModule == null)
            {
                return (T)PrefabUtility.InstantiatePrefab(modulePrefab, interactiveObjectType.transform);
            }
            return foundPrefabModule;
        }

        public static void RemovePrefabModule<T>(InteractiveObjectType interactiveObjectType) where T : InteractiveObjectModule
        {
            T foundPrefabModule = interactiveObjectType.GetComponentInChildren<T>();
            if (foundPrefabModule != null)
            {
                MonoBehaviour.DestroyImmediate(foundPrefabModule.gameObject);
            }
        }

    }
}