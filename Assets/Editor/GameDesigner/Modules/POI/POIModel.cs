using UnityEngine;
using System.Collections;
using UnityEditor;
using AdventureGame;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class POIModel : IGameDesignerModule
    {
        private GameObject POIModelObject;

        public void GUITick()
        {
            var currentSelectedObj = GameDesignerHelper.GetCurrentSceneSelectedObject();

            this.POIModelObject = (GameObject)EditorGUILayout.ObjectField("POI new Model : ", this.POIModelObject, typeof(GameObject), false);

            EditorGUI.BeginDisabledGroup(this.POIModelObject == null || currentSelectedObj == null || currentSelectedObj.GetComponent<PointOfInterestType>() == null);
            if (GUILayout.Button("SET POI MODEL"))
            {
                PrefabUtility.InstantiatePrefab(this.POIModelObject, currentSelectedObj.transform.parent.gameObject.FindChildObjectRecursively("Model").transform);

            }
            EditorGUI.EndDisabledGroup();
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}