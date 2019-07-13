using RTPuzzle;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    public static class EditorInteractiveObjectModulesOperation
    {

        public static void AddPrefabModule<T>(InteractiveObjectType interactiveObjectType, T modulePrefab) where T : InteractiveObjectModule
        {
            T foundPrefabModule = interactiveObjectType.GetComponentInChildren<T>();
            if (foundPrefabModule == null)
            {
                PrefabUtility.InstantiatePrefab(modulePrefab, interactiveObjectType.transform);
            }
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