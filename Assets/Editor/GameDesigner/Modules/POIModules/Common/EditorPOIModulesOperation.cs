using AdventureGame;
using CoreGame;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    public static class EditorPOIModulesOperation
    {

        public static void AddModule<T>(PointOfInterestType PointOfInterestType) where T : APointOfInterestModule
        {
            var moduleObject = PointOfInterestTypeHelper.GetModulesObject(PointOfInterestTypeHelper.GetPointOfInterestModules(PointOfInterestType));
            if (moduleObject.GetComponent<T>() == null)
            {
                moduleObject.AddComponent<T>();
            }
        }

        public static void AddPrefabModule<T>(PointOfInterestType PointOfInterestType, T modulePrefab) where T : APointOfInterestModule
        {
            var pointOfInterestModules = PointOfInterestTypeHelper.GetPointOfInterestModules(PointOfInterestType);
            T foundPrefabModule = pointOfInterestModules.GetComponentInChildren<T>();
            if (foundPrefabModule == null)
            {
                PrefabUtility.InstantiatePrefab(modulePrefab, pointOfInterestModules.transform);
            }
        }

        public static void RemoveModule<T>(PointOfInterestType PointOfInterestType) where T : APointOfInterestModule
        {
            var moduleObject = PointOfInterestTypeHelper.GetModulesObject(PointOfInterestTypeHelper.GetPointOfInterestModules(PointOfInterestType));
            if (moduleObject.GetComponent<T>() != null)
            {
                MonoBehaviour.DestroyImmediate(moduleObject.GetComponent<T>());
            }
        }

        public static void RemovePrefabModule<T>(PointOfInterestType PointOfInterestType) where T : APointOfInterestModule
        {
            var pointOfInterestModules = PointOfInterestTypeHelper.GetPointOfInterestModules(PointOfInterestType);
            T foundPrefabModule = pointOfInterestModules.GetComponentInChildren<T>();
            if (foundPrefabModule != null)
            {
                MonoBehaviour.DestroyImmediate(foundPrefabModule.gameObject);
            }
        }

        public static void AddDataComponent<T>(PointOfInterestType PointOfInterestType) where T : ADataComponent
        {
            var dataComponentObject = PointOfInterestTypeHelper.GetDataComponentsObject(PointOfInterestTypeHelper.GetDataComponentContainer(PointOfInterestType));
            if (dataComponentObject.GetComponent<T>() == null)
            {
                dataComponentObject.AddComponent<T>();
            }
        }

        public static void RemoveDataComponent<T>(PointOfInterestType PointOfInterestType) where T : ADataComponent
        {
            var dataComponentObject = PointOfInterestTypeHelper.GetDataComponentsObject(PointOfInterestTypeHelper.GetDataComponentContainer(PointOfInterestType));
            if (dataComponentObject.GetComponent<T>() != null)
            {
                MonoBehaviour.DestroyImmediate(dataComponentObject.GetComponent<T>());
            }
        }
    }
}