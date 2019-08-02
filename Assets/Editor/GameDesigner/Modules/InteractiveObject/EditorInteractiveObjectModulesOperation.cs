using RTPuzzle;
using System;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    public static class EditorInteractiveObjectModulesOperation
    {

        public static InteractiveObjectModule AddPrefabModule(InteractiveObjectType interactiveObjectType, InteractiveObjectModule modulePrefab) 
        {
            InteractiveObjectModule foundPrefabModule = (InteractiveObjectModule)interactiveObjectType.GetComponentInChildren(modulePrefab.GetType());
            if (foundPrefabModule == null)
            {
                return (InteractiveObjectModule)PrefabUtility.InstantiatePrefab(modulePrefab, interactiveObjectType.transform);
            }
            return foundPrefabModule;
        }

        public static void RemovePrefabModule(InteractiveObjectType interactiveObjectType, Type moduleType)
        {
            InteractiveObjectModule foundPrefabModule = (InteractiveObjectModule)interactiveObjectType.GetComponentInChildren(moduleType);
            if (foundPrefabModule != null)
            {
                MonoBehaviour.DestroyImmediate(foundPrefabModule.gameObject);
            }
        }

    }
}